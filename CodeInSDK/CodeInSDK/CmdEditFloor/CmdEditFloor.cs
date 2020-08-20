using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace CodeInSDK.CmdEditFloor
{
    [Transaction(TransactionMode.Manual)]
    class CmdEditFloor : IExternalCommand
    {
    #region Super simple floor creation
        public Result Execute2(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Create a Floor");

                int n = 4;
                XYZ[] points = new XYZ[n];
                points[0] = XYZ.Zero;
                points[1] = new XYZ(10, 0, 0);
                points[2] = new XYZ(10, 10, 0);
                points[3] = new XYZ(0, 10, 0);

                CurveArray curve = new CurveArray();
                for (int i = 0; i < n; i++)
                {
                    Line line = Line.CreateBound(points[i], points[(i < n - 1) ? i + 1 : 0]);
                    curve.Append(line);
                }
                doc.Create.NewFloor(curve, true);
                tx.Commit();
            }
            return Result.Succeeded;
        }
    #endregion // Super simple floor creation

        /// <summary>
        /// Return the uppermost horizontal face of a given "horizontal " solid object such as a floor slab. Currently only supports planar face.
        /// </summary>
        /// <param name="solid"></param>
        /// <returns></returns>
        PlanarFace GetTopFace(Solid solid)
        {
            PlanarFace topFace = null;
            FaceArray faces = solid.Faces;
            foreach (Face f in faces)
            {
                PlanarFace pf = f as PlanarFace;
                if (null != pf && Util.IsHorizontal(pf))
                {
                    if ((null == topFace) || (topFace.Origin.Z < pf.Origin.Z))
                    {
                        topFace = pf;
                    }
                }
            }
            return topFace;
        }

        public Result Execute
        (
            ExternalCommandData commandData, ref string message,
            ElementSet elements
        )
        {
            UIApplication app = commandData.Application;
            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = uidoc.Document;

            //Retrieve selected floors, or all floors, if nothing is selected : 
            List<Element> floors = new List<Element>();
            if (!Util.GetSelectedElementsOrAll(floors, uidoc, typeof(Floor)))
            {
                Selection sel = uidoc.Selection;
                message = (0 < sel.GetElementIds().Count)
                    ? "Please select some floor elements."
                    : "No floor elements found. ";
                return Result.Failed;
            }

            //Determine top face of each selected floor :
            int nNullFaces = 0;
            List<Face> topFaces = new List<Face>();
            Options opt = app.Application.Create.NewGeometryOptions();

            foreach (Floor floor in floors)
            {
                GeometryElement geo = floor.get_Geometry(opt);

                foreach (GeometryObject obj in geo)
                {
                    Solid solid = obj as Solid;
                    if (solid != null)
                    {
                        PlanarFace f = GetTopFace(solid);
                        if (null == f)
                        {
                            Debug.WriteLine(Util.ElementDescription(floor) + " has no top face. ");
                            ++nNullFaces;
                        }
                        topFaces.Add(f);
                    }
                }
            }

            using (Transaction t = new Transaction(doc))
            {
                t.Start("Create Model Lines and Floor");

                //Create new floors from the top faces found.
                //Before creating the new floor, we would obviously apply whatever modifications are required to the new floor profile:
                Autodesk.Revit.Creation.Application creApp = app.Application.Create;
                Autodesk.Revit.Creation.Document creDoc = doc.Create;

                int i = 0;
                int n = topFaces.Count - nNullFaces;

                Debug.Print("{0} top face{1} found.", n, Util.PluralSuffix(n));

                foreach (Face f in topFaces)
                {
                    Floor floor = floors[i++] as Floor;
                    if (null != f)
                    {
                        EdgeArrayArray eaa = f.EdgeLoops;
                        CurveArray profile;
                        {
                            profile = new CurveArray();
                            // only use first edge array,
                            //the outer boundary loop,
                            //skip the further items,
                            //representing holes.

                            EdgeArray ea = eaa.get_Item(0);
                            foreach (Edge edge in ea)
                            {
                                IList<XYZ> pts = edge.Tessellate();
                                int m = pts.Count;
                                XYZ p = pts[0];
                                XYZ q = pts[m - 1];
                                Line line = Line.CreateBound(p, q);
                                profile.Append(line);
                            }
                        }

                        //
                        Level level = doc.GetElement(floor.LevelId) as Level;
                        floor = creDoc.NewFloor(profile, floor.FloorType, level, true);
                        XYZ v = new XYZ(5, 5, 0);
                        ElementTransformUtils.MoveElement(doc, floor.Id, v);
                    }
                }
                t.Commit();
            }
            return Result.Succeeded;
        }

    #region Set Floor Level and Offset
        void SetFloorLevelAndOffset(Document doc)
        {
            Floor floor =new FilteredElementCollector(doc).WhereElementIsNotElementType().OfCategory(BuiltInCategory.OST_Floors).OfClass(typeof(Floor)).FirstElement() as Floor;
            
            //Get first level not used by floor

            int levelIdInt = floor.LevelId.IntegerValue;
            Element level =new FilteredElementCollector(doc).WhereElementIsNotElementType().OfCategory(BuiltInCategory.OST_Levels).OfClass(typeof(Level)).FirstOrDefault<Element>(e =>e.Id.IntegerValue.Equals(levelIdInt));
            if (null != level)
            {
                Parameter p = floor.get_Parameter(BuiltInParameter.LEVEL_PARAM);

                Parameter p1 = floor.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM);

                using (Transaction tx =new Transaction(doc))
                {
                    tx.Start("Set floor Level");
                    p.Set(level.Id); //set new level Id
                    p1.Set(2); //Set new offset from level
                    tx.Commit();
                }
            }
        }
    #endregion //set floor level and offset.
    }
}