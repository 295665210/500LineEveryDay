using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

// code in BuildingCoder
namespace CodeInSDK.CmdDimensionWallsFindRefs
{
    [Transaction(TransactionMode.Manual)]
    class CmdDimensionWallsIterateFaces : IExternalCommand
    {
#region Developer Guide Sample Code
        public void DuplicateDimension(Document doc, Dimension dimension)
        {
            Line line = dimension.Curve as Line;
            if (null != line)
            {
                View view = dimension.View;
                ReferenceArray references = dimension.References;
                Dimension newDimension = doc.Create.NewDimension(view, line, references);
            }
        }

        public Dimension CreateLinearDimension(Document doc)
        {
            Application app = doc.Application;
            //first create two lines
            XYZ pt1 = new XYZ(5, 5, 0);
            XYZ pt2 = new XYZ(5, 10, 0);
            Line line = Line.CreateBound(pt1, pt2);

            Plane plane = Plane.CreateByNormalAndOrigin(pt1.CrossProduct(pt2), pt2);
            SketchPlane sketchPlane = SketchPlane.Create(doc, plane);
            ModelCurve modelCurve1 = doc.FamilyCreate.NewModelCurve(line, sketchPlane);

            pt1 = new XYZ(10, 5, 0);
            pt2 = new XYZ(10, 10, 0);
            line = Line.CreateBound(pt1, pt2);
            plane = Plane.CreateByNormalAndOrigin(pt1.CrossProduct(pt2), pt2);

            sketchPlane = SketchPlane.Create(doc, plane);

            ModelCurve modelCurve2 = doc.FamilyCreate.NewModelCurve(line, sketchPlane);

            //now create a linear dimension between them
            ReferenceArray ra = new ReferenceArray();
            ra.Append(modelCurve1.GeometryCurve.Reference);
            ra.Append(modelCurve2.GeometryCurve.Reference);

            pt1 = new XYZ(5, 10, 0);
            pt2 = new XYZ(10, 10, 0);
            line = Line.CreateBound(pt1, pt2);
            Dimension dim = doc.FamilyCreate.NewLinearDimension(doc.ActiveView, line, ra);

            //create a label for the dimension called "width"
            FamilyParameter param = doc.FamilyManager.AddParameter("width", BuiltInParameterGroup.PG_CONSTRAINTS,
                ParameterType.Length, false);
            dim.FamilyLabel = param;
            return dim;
        }
#endregion

        private const string _prompt = "Please selcct two parallel opppsing straght walls.";

#region Create Dimension Element
        public static void CreateDimensionElement(View view, XYZ p1, Reference r1, XYZ p2, Reference r2)
        {
            Document doc = view.Document;
            ReferenceArray ra = new ReferenceArray();
            ra.Append(r1);
            ra.Append(r2);
            Line line = Line.CreateBound(p1, p2);
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Create new Dimension");
                Dimension dim = doc.Create.NewDimension(view, line, ra);

                t.Commit();
            }
        }
#endregion

#region GetColsetFace
        static Face GetClosestFace(Element e, XYZ p, XYZ normal, Options opt)
        {
            Face face = null;
            double min_distance = double.MaxValue;
            GeometryElement geo = e.get_Geometry(opt);

            foreach (GeometryObject obj in geo)
            {
                Solid solid = obj as Solid;
                if (solid != null)
                {
                    FaceArray fa = solid.Faces;
                    foreach (Face f in fa)
                    {
                        PlanarFace pf = f as PlanarFace;
                        Debug.Assert(null != pf, "excepted planar wall faces");

                        if (null != pf && Util.IsParallel(normal, pf.FaceNormal))
                        {
                            XYZ v = p - pf.Origin;
                            double d = v.DotProduct(-pf.FaceNormal);
                            if (d < min_distance)
                            {
                                face = f;
                                min_distance = d;
                            }
                        }
                    }
                }
            }
            return face;
        }
#endregion


        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            ICollection<ElementId> ids = uidoc.Selection.GetElementIds();
            List<Wall> walls = new List<Wall>(2);
            foreach (ElementId id in ids)
            {
                Element e = doc.GetElement(id);
                if (e is Wall)
                {
                    walls.Add(e as Wall);
                }
            }

            if (2 != walls.Count)
            {
                message = _prompt;
                return Result.Failed;
            }

            List<Line> lines = new List<Line>(2);
            List<XYZ> midPoints = new List<XYZ>(2);
            XYZ normal = null;

            foreach (Wall wall in walls)
            {
                LocationCurve lc = wall.Location as LocationCurve;
                Curve curve = lc.Curve;

                if (!(curve is Line))
                {
                    message = _prompt;
                    return Result.Failed;
                }
                Line l = curve as Line;
                lines.Add(l);
                midPoints.Add(Util.Midpoint(l));

                if (null == normal)
                {
                    normal = Util.Normal(l);
                }
                else
                {
                    if (!Util.IsParallel(normal,Util.Normal(l)))
                    {
                        message = _prompt;
                        return Result.Failed;
                    }
                }
            }

            //find the two closet facing  faces on the walls;
            //they are vertical faces that are parallel to the
            //wall curve and closet to the other wall.

            Options opt = app.Create.NewGeometryOptions();
            opt.ComputeReferences = true;

            List<Face> faces = new List<Face>(2);
            faces.Add(GetClosestFace(walls[0],midPoints[1],normal,opt));
        }
    }
}