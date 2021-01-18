// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
// using Autodesk.Revit.Attributes;
// using Autodesk.Revit.DB;
// using Autodesk.Revit.DB.Architecture;
// using Autodesk.Revit.UI;
//
// namespace CodeInBuildingCoder1
// {
//     [Transaction(TransactionMode.Manual)]
//     class CmdCropToRoom : IExternalCommand
//     {
//         /// <summary>
//         /// Set 3D view section box to selected element extents.
//         /// </summary>
//         /// <param name="uidoc"></param>
//         private void SectionBox(UIDocument uidoc)
//         {
//             Document doc = uidoc.Document;
//             View view = doc.ActiveView;
//
//             double Min_X = double.MaxValue;
//             double Min_Y = double.MaxValue;
//             double Min_Z = double.MaxValue;
//
//             double Max_X = Min_X;
//             double Max_Y = Min_Y;
//             double Max_Z = Min_Z;
//
//             ICollection<ElementId> ids = uidoc.Selection.GetElementIds();
//
//             foreach (ElementId id in ids)
//             {
//                 Element elm = doc.GetElement(id);
//                 BoundingBoxXYZ box = elm.get_BoundingBox(view);
//                 Max_X = (box.Max.X > Max_X) ? box.Max.X : Max_X;
//                 Max_Y = (box.Max.Y > Max_Y) ? box.Max.Y : Max_Y;
//                 Max_Z = (box.Max.Z > Max_Z) ? box.Max.Z : Max_Z;
//
//                 Min_X = (box.Min.X < Min_X) ? box.Min.X : Min_X;
//                 Min_Y = (box.Min.Y < Min_Y) ? box.Min.X : Min_Y;
//                 Min_Z = (box.Min.Z < Min_Z) ? box.Min.X : Min_Z;
//             }
//
//             XYZ Max = new XYZ(Max_X, Max_Y, Max_Z);
//             XYZ Min = new XYZ(Min_X, Min_Y, Min_Z);
//
//             BoundingBoxXYZ myBox = new BoundingBoxXYZ();
//             myBox.Min = Min;
//             myBox.Max = Max;
//             (view as View3D).SetSectionBox(myBox);
//         }
//
//
//         private static int _i = -1;
//
//         /// <summary>
//         /// Increment and return the current room index.
//         /// Every call to this method increments the current room index by one.
//         /// If it exceeds the number of rooms in the model, loop back to zero.
//         /// </summary>
//         /// <param name="room_count"></param>
//         /// <returns></returns>
//         static int BumpRoomIndex(int room_count)
//         {
//             ++_i;
//             if (_i >= room_count)
//             {
//                 _i = 0;
//             }
//             return _i;
//         }
//
//
//         public Result Execute(
//             ExternalCommandData commandData, ref string message,
//             ElementSet elements)
//         {
//             UIApplication uiapp = commandData.Application;
//             Document doc = uiapp.ActiveUIDocument.Document;
//             View3D view3D = doc.ActiveView as View3D;
//
//             if (null == view3D)
//             {
//                 message = " Please activate a 3D view" +
//                           " before running the command.";
//                 return Result.Failed;
//             }
//
//             using (Transaction t = new Transaction(doc))
//             {
//                 t.Start("Crop to room");
//                 //get the 3d view crop box;
//                 BoundingBoxXYZ bb = view3D.CropBox;
//
//                 //get the transform from the current view to the 3d model;
//                 Transform transform = bb.Transform;
//
//                 //get the transform from the 3D model to the current view;
//                 Transform transformInverse = transform.Inverse;
//
//                 //get all rooms in the model
//                 FilteredElementCollector collector =
//                     new FilteredElementCollector(doc);
//
//                 collector.OfClass(typeof(SpatialElement)).OfCategory(BuiltInCategory.OST_Rooms);
//                 IList<Element> rooms = collector.ToElements();
//                 int n = rooms.Count;
//
//                 Room room = (0 < 1) ? rooms[BumpRoomIndex(n)] as Room : null;
//
//                 if (null == room)
//                 {
//                     message = "No room element found in project.";
//                     return Result.Failed;
//                 }
//
//                 //collect all vertices of room closed shell
//                 //to determine the extents.
//                 GeometryElement e = room.ClosedShell;
//                 List<XYZ> vertices = new List<XYZ>();
//
//                 foreach (GeometryObject o in e)
//                 {
//                     if (o is Solid)
//                     {
//                         Solid solid = o as Solid;
//                         foreach (Edge edge in solid.Edges)
//                         {
//                             foreach (XYZ p in edge.Tessellate())
//                             {
//                                 vertices.Add(p);
//                             }
//                         }
//                     }
//                 }
//
//                 List<XYZ> verticesIn3dView = new List<XYZ>();
//                 foreach (XYZ p in vertices)
//                 {
//                     verticesIn3dView.Add(transformInverse.OfPoint(p));
//                 }
//
//                 //Ignore the Z coorindates and fine the 
//                 //min and max X and Y in the 3d view;
//                 double xMin = 0, yMin = 0, xMax = 0, yMax = 0;
//                 bool first = true;
//                 foreach (XYZ p in verticesIn3dView)
//                 {
//                     if (first)
//                     {
//                         xMin = p.X;
//                         yMin = p.Y;
//                         xMax = p.X;
//                         yMax = p.Y;
//                         first = false;
//                     }
//                     else
//                     {
//                         if (xMin > p.X)
//                         {
//                             xMin = p.X;
//                         }
//                         if (yMin > p.Y)
//                         {
//                             yMin = p.Y;
//                         }
//                         if (xMax < p.X)
//                         {
//                             xMax = p.X;
//                         }
//                         if (yMax < p.Y)
//                         {
//                             yMax = p.Y;
//                         }
//                     }
//                 }
//
//                 //Grow the crop box by one twentieth of its
//                 //size to include the walls of the room;
//                 double d = 0.05 * (xMax - xMin);
//                 xMin = xMin - d;
//                 xMax = xMin + d;
//
//                 d = 0.05 * (yMax - yMin);
//                 yMin = yMin - d;
//                 yMax = yMax + d;
//
//                 bb.Max = new XYZ(xMax, yMax, bb.Max.Z);
//                 bb.Min = new XYZ(xMin, yMin, bb.Min.Z);
//
//                 view3D.CropBox = bb;
//
//                 //change the crop view setting manually or
//                 //programmatically to see the result.
//                 view3D.CropBoxActive = true;
//                 view3D.CropBoxVisible = true;
//                 t.Commit();
//             }
//             return Result.Succeeded;
//         }
//
//         public static void AdjustViewCropToSectionBox(View3D view)
//         {
//             if (!view.IsSectionBoxActive)
//             {
//                 return;
//             }
//             if (!view.CropBoxActive)
//             {
//                 view.CropBoxActive = true;
//             }
//             BoundingBoxXYZ CropBox = view.CropBox;
//             BoundingBoxXYZ SectionBox = view.GetSectionBox();
//             Transform T = CropBox.Transform;
//             var corners = BBCorners(SectionBox, T);
//             double MinX = corners.Min(j => j.X);
//             double MinY = corners.Min(j => j.Y);
//             double MinZ = corners.Min(j => j.Z);
//
//             double MaxX = corners.Max(j => j.X);
//             double MaxY = corners.Max(j => j.Y);
//             double MaxZ = corners.Max(j => j.Z);
//
//             CropBox.Min = new XYZ(MinX, MinY, MinZ);
//             CropBox.Max = new XYZ(MaxX, MaxY, MaxZ);
//             view.CropBox = CropBox;
//         }
//
//
//         // Set View Cropbox to Section Box 
//         // https://forums.autodesk.com/t5/revit-api-forum/set-view-cropbox-to-a-section-box/m-p/9600049
//         private static XYZ[] BBCorners(BoundingBoxXYZ SectionBox, Transform T)
//         {
//             XYZ sbmn = SectionBox.Min;
//             XYZ sbmx = SectionBox.Max;
//             XYZ Btm_LL = sbmn;                            // Lower Left
//             var Btm_LR = new XYZ(sbmx.X, sbmn.Y, sbmn.Z); // Lower Right
//             var Btm_UL = new XYZ(sbmn.X, sbmx.Y, sbmn.Z); // Upper Left
//             var Btm_UR = new XYZ(sbmx.X, sbmx.Y, sbmn.Z); // Upper Right
//
//             XYZ Top_UR = sbmx;                            // Upper Right
//             var Top_UL = new XYZ(sbmn.X, sbmx.Y, sbmx.Z); // Upper Left
//             var Top_LR = new XYZ(sbmx.X, sbmn.Y, sbmx.Z); // Lower Right
//             var Top_LL = new XYZ(sbmn.X, sbmn.Y, sbmx.Z); // Lower Left
//
//             var Out = new XYZ[8]
//             {
//                 Btm_LL, Btm_LR, Btm_UL, Btm_UR,
//                 Top_UR, Top_UL, Top_LR, Top_LL
//             };
//
//             for (int i = 0, loopTo = Out.Length - 1; i < loopTo; i++)
//             {
//                 //Transform bounding box coords to model coords
//                 Out[i] = SectionBox.Transform.OfPoint(Out[i]);
//                 //Transform bounding box coords to view coords
//                 Out[i] = T.Inverse.OfPoint(Out[i]);
//             }
//             return Out;
//         }
//     }
// }