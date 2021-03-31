using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;

namespace CodeInBuildingCoder1
{
    [Transaction(TransactionMode.Manual)]
    class CmdExteriorWalls : IExternalCommand
    {
        /// <summary>
        /// Return a bounding box around all the
        /// walls in the entire model;for just a
        /// building,or several buildings ,this is
        /// obviously equal to the model extents.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="view"></param>
        /// <returns></returns>
        static BoundingBoxXYZ getBoundingBoxAroundAllWalls(
            Document doc, View view = null)
        {
            //Default constructor creates cube from -100 to 100;
            //maybe too big ,but who cares?
            BoundingBoxXYZ bb = new BoundingBoxXYZ();
            FilteredElementCollector walls =
                new FilteredElementCollector(doc).OfClass(typeof(Wall));
            foreach (Wall wall in walls)
            {
                bb.ExpandToContain(wall.get_BoundingBox(view));
            }
            return bb;
        }

        static List<ElementId> RetrieveWallsGeneratingRoomBoundaries(
            Document doc, Room room)
        {
            List<ElementId> ids = new List<ElementId>();
            IList<IList<BoundarySegment>> boundaries =
                room.GetBoundarySegments(new SpatialElementBoundaryOptions());
            int n = boundaries.Count;
            int iBoundary = 0;
            int iSegment;
            foreach (IList<BoundarySegment> b in boundaries)
            {
                ++iBoundary;
                iSegment = 0;
                foreach (BoundarySegment s in b) //BoundarySegment:段，部分
                {
                    ++iSegment;
                    //Retrieve the id of the element that
                    //produces this boundary segment
                    Element neighbour = doc.GetElement(s.ElementId);
                    Curve curve = s.GetCurve();
                    double length = curve.Length;

                    if (neighbour is Wall)
                    {
                        Wall wall = neighbour as Wall;
                        Parameter p =
                            wall.get_Parameter(BuiltInParameter
                                                   .HOST_AREA_COMPUTED);
                        double area = p.AsDouble();
                        LocationCurve lc = wall.Location as LocationCurve;
                        double wallLength = lc.Curve.Length;
                        ids.Add(wall.Id);
                    }
                }
            }
            return ids;
        }

        /// <summary>
        /// 获取当前模型指定视图内的所有最外层的墙体
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="view"></param>
        /// <returns></returns>
        public static List<ElementId> GetOutmostWalls(
            Document doc, View view = null)
        {
            double offset = Util.MmToFoot(1000);
            if (view == null)
            {
                view = doc.ActiveView;
            }
            //Obsolete 废弃的代码
            BoundingBoxXYZ bb = getBoundingBoxAroundAllWalls(doc, view);
            XYZ voffset = offset * (XYZ.BasisZ + XYZ.BasisY);
            bb.Min -= voffset;
            bb.Max += voffset;
            XYZ[] bottom_corners = Util.GetBottomCorners(bb, 0);
            CurveArray curves = new CurveArray();
            for (int i = 0; i < 4; ++i)
            {
                int j = i < 3 ? i + 1 : 0;
                curves.Append(Line.CreateBound(bottom_corners[i],
                                               bottom_corners[j]));
            }
            using (TransactionGroup group = new TransactionGroup(doc))
            {
                Room newRoom = null;
                group.Start("Find Outermost Walls");
                using (Transaction transaction = new Transaction(doc))
                {
                    transaction.Start("Create new Room Boundary Lines");
                    SketchPlane sketchPlane =
                        SketchPlane.Create(doc, view.GenLevel.Id);
                    ModelCurveArray modelCaRoomBoundaryLines =
                        doc.Create.NewRoomBoundaryLines(sketchPlane, curves,
                         view);
                    //创建房间的坐标点 create room coordinates
                    double d = Util.MmToFoot(600);
                    UV point = new UV(bb.Min.X + d, bb.Min.Y + d);
                    //根据选中点，创建房间 当前视图的楼层 
                    newRoom = doc.Create.NewRoom(view.GenLevel, point);
                    if (newRoom == null)
                    {
                        string msg = "创建房间失败。";
                        TaskDialog.Show("xxx", msg);
                        transaction.RollBack();
                        return null;
                    }
                    RoomTag tag =
                        doc.Create.NewRoomTag(new LinkElementId(newRoom.Id),
                                              point, view.Id);
                    transaction.Commit();
                }

                //获取房间墙体
                List<ElementId> ids =
                    RetrieveWallsGeneratingRoomBoundaries(doc, newRoom);
                group.RollBack(); //撤销
                return ids;
            }
        }


        public Result Execute(
            ExternalCommandData commandData, ref string message,
            ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            List<ElementId> ids = GetOutmostWalls(doc);
            uidoc.Selection.SetElementIds(ids);
            return Result.Succeeded;
        }

        List<ElementId> GetElementIdsFromString(string x)
        {
            return new List<ElementId>(x.Split('\n')
                                           .Select<string, ElementId>
                                               (s => new ElementId(int.Parse(s))));
        }
    }
}