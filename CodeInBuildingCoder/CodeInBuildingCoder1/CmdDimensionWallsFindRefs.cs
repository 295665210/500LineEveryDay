#region Header
#endregion //Header


#region namespace
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using BuildingCoder;
#endregion //namespace

namespace CodeInBuildingCoder1
{
    /// <summary>
    /// Dimension two opposing parallel walls.  //相对的反向的平行墙。
    /// Prompt user to select the first wall.and
    /// the second at the point at which to create
    /// the dimensioning . Use FindReferencesByDirection to determine the wall face references.
    ///
    /// Second sample solution for case 
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    class CmdDimensionWallsFindRefs : IExternalCommand
    {
        private const string _prompt = //prompt :提示
            "Please select two parallel straight walls" + " with a partial projected overlap.";

        #region Get3DView
        private View3D Get3DView(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(View3D));
            foreach (View3D v in collector)
            {
                //skip view templates here because they are invisible in project browsers:
                if (v != null && !v.IsTemplate && v.Name == "{3D}")
                {
                    return v;
                }
            }
            return null;
        }
        #endregion //Get3DView

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            //select two walls and the dimension line point:
            Selection sel = uidoc.Selection;
            ReferenceArray refs = new ReferenceArray();

            try
            {
                ISelectionFilter f = new JtElementsOfClassSelectionFilter<Wall>();

                refs.Append(sel.PickObject(ObjectType.Element, f, "Please select first wall."));

                refs.Append(sel.PickObject(ObjectType.Element, f,
                                           "Please pic dimension line" + " point on second wall"));
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                message = "No two walls selected.";
                return Result.Failed;
            }

            //Ensure the two selected walls are straight and parallel;
            //determine their mutual(相互的) normal vector and a 
            //point on each wall for distance calculations.
            Wall[] walls = new Wall[2];
            List<int> ids = new List<int>(2);
            XYZ[] pts = new XYZ[2];
            Line[] lines = new Line[2];
            IntersectionResult ir;

            //摘要:
            //     This class captures results of intersecting geometric entities. "Intersecting"
            //     is meant in generalized sense, so the same class will be used for projection,
            //     containment, etc. Refer to the documentation of the method providing the result
            //     for the precise meaning of properties.
            //该类捕获相交的几何实体的结果。“相交”是广义意义上的，所以同样的类别将用于投影，容器等。
            //参考提供结果的方法的文档属性的精确含义。
            XYZ normal = null;
            int i = 0;

            foreach (Reference r in refs)
            {
                Wall wall = doc.GetElement(r) as Wall;
                ids.Add(wall.Id.IntegerValue);

                //Obtain location curve and check that it is straight.
                LocationCurve lc = wall.Location as LocationCurve;
                Curve curve = lc.Curve;
                lines[i] = curve as Line;

                if (null == lines[i])
                {
                    message = _prompt;
                    return Result.Failed;
                }
                //Obtain normal vectors and ensure that they are equal,
                //i.e. walls are parallel;

                if (null == normal)
                {
                    normal = Util.Normal(lines[i]);
                }

                else
                {
                    if (!Util.IsParallel(normal, Util.Normal(lines[i])))
                    {
                        message = _prompt;
                        return Result.Failed;
                    }
                }

                //Obtain pick points and project  onto wall  location lines://Obtain(获得)
                XYZ p = r.GlobalPoint;
                ir = lines[i].Project(p);

                if (null == ir)
                {
                    message = string.Format("Unable to project pick point {0}" + "onto wall location line.", i);
                    return Result.Failed;
                }

                pts[i] = ir.XYZPoint;

                Debug.Print(
                            "Wall {0} id {1} at {2}, {3} --> point {4}",
                            i, wall.Id.IntegerValue,
                            Util.PointString(lines[i].GetEndPoint(0)),
                            Util.PointString(lines[i].GetEndPoint(1)),
                            Util.PointString(pts[i]));

                if (0 < i)
                {
                    ir = lines[0].Project(pts[1]);
                    if (null == ir)
                    {
                        message = string.Format("Unable to project selected dimension "
                                                + "line point {0} on second wall onto "
                                                + "first wall's location line.",
                                                Util.PointString(pts[1]));
                        return Result.Failed;
                    }
                    pts[0] = ir.XYZPoint;
                }
                ++i;
            }

            XYZ v = pts[0] - pts[1];
            if (0 > v.DotProduct(normal))
            {
                normal = -normal;
            }

            //shoot a ray back from the second
            //picked wall towards first:
            Debug.Print(
                        "Shooting ray from {0} in direction {1}",
                        Util.PointString(pts[1]),
                        Util.PointString(normal));

            View3D view = Get3DView(doc);
            if (null == view)
            {
                message = "No 3D view named '{3D}' found; "
                          + "run the View > 3D View command once "
                          + "to generate it.";
                return Result.Failed;
            }

            ReferenceIntersector ri = new ReferenceIntersector(walls[0].Id, FindReferenceTarget.Element, view);
            ReferenceWithContext ref2 = ri.FindNearest(pts[1], normal);

            if (null == ref2)
            {
                message = "ReferenceIntersector.FindNearest"
                          + " returned null!";
                return Result.Failed;
            }

            CmdDimensionWallsIterateFaces.CreateDimensionElement(doc.ActiveView, pts[0], ref2.GetReference(), pts[1],
                                                                 refs.get_Item(1));
            return Result.Succeeded;
        }

        #region Find ceiling face to place light fixture.
        /// <summary>
        /// Return reference to ceiling face to place lighting fixture above a given point.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        Reference GetCeilingReferenceAbove(View3D view, XYZ p)
        {
            ElementClassFilter filter = new ElementClassFilter(typeof(Ceiling));
            ReferenceIntersector refIntersector = new ReferenceIntersector(filter, FindReferenceTarget.Face, view);

            refIntersector.FindReferencesInRevitLinks = true;

            ReferenceWithContext rwc = refIntersector.FindNearest(p, XYZ.BasisZ);

            Reference r = (null == rwc) ? null : rwc.GetReference();

            if (null == r)
            {
                System.Windows.MessageBox.Show("no intersecting geometry");
            }
            return r;
        }

        void TestGetCeilingReferenceAbove(Document doc)
        {
            View3D view = doc.GetElement(new ElementId(147335)) as View3D;
            Space space = doc.GetElement(new ElementId(151759)) as Space;
            XYZ center = ((LocationPoint) space.Location).Point;

            Reference r = GetCeilingReferenceAbove(view, center);

            //populate these as needed:  ///populate 居住于
            XYZ startPoint = null;
            FamilySymbol sym = null;
            doc.Create.NewFamilyInstance(r, startPoint, XYZ.BasisZ, sym);
        }
        #endregion // Find ceiling face to place light fixture
    }
}