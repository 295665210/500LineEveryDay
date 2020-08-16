// using System;
// using System.Collections.Generic;
// using System.Diagnostics;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
// using Autodesk.Revit.Attributes;
// using Autodesk.Revit.DB;
// using Autodesk.Revit.UI;
//
/////代码有问题，会让revit程序崩溃。
// namespace CodeInSDK.CmdBoundingBox
// {
//     [Transaction(TransactionMode.Manual)]
//     class CmdBoundingBox : IExternalCommand
//
//     {
//         //XYZ wrapper class implementing IComparable.
//
//         class XyzComparable : XYZ, IComparable<XYZ>
//         {
//             public XyzComparable(XYZ a) : base(a.X, a.Y, a.Z)
//             {
//             }
//
//             int IComparable<XYZ>.CompareTo(XYZ a)
//             {
//                 return Util.Compare(this, a);
//             }
//         }
//
//
//         private static BoundingBoxXYZ RotatedBoundingBox(BoundingBoxXYZ b, Transform t)
//         {
//             double height = b.Max.Z - b.Min.Z;
//             //four corners : lower left ,lower right.
//             //upper right , upper left :
//             XYZ[] corners = Util.GetBottomCorners(b);
//
//             XyzComparable[] cornersTransformed =
//                 corners.Select<XYZ, XyzComparable>(p => new XyzComparable(t.OfPoint(p))).ToArray();
//
//             b.Min = cornersTransformed.Min();
//             b.Max = cornersTransformed.Max();
//             b.Max += height * XYZ.BasisZ;
//
//             return b;
//         }
//
//
//         public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
//         {
//             UIApplication app = commandData.Application;
//             UIDocument uidoc = app.ActiveUIDocument;
//             Document doc = uidoc.Document;
//
//             Element e = Util.SelectSingleElement(uidoc, "An element");
//             if (null == e)
//             {
//                 message = " No element selected";
//                 return Result.Failed;
//             }
//
//             View v = null;
//             BoundingBoxXYZ b = e.get_BoundingBox(v);
//             if (null == b)
//             {
//                 v = commandData.View;
//                 b = e.get_BoundingBox(v);
//             }
//
//             else
//             {
//                 using (Transaction tx = new Transaction(doc))
//                 {
//                     tx.Start("Draw Model Line Bounding Box Outline");
//                     Debug.Assert(b.Transform.IsIdentity, "Expected identity bounding box transform");
//                     string in_view = (null == v) ? "model space" : "view" + v.Name;
//
//                     Util.InfoMsg(string.Format("Element bounding box of {0} in " + "{1} extends form {2} to {3}.",
//                         Util.ElementDescription(e), in_view, Util.PointString(b.Min), Util.PointString(b.Max)));
//                     Creator creator = new Creator(doc);
//
//                     creator.DrawPolygon(new List<XYZ>(Util.GetBottomCorners(b)));
//
//                     Transform rotation = Transform.CreateRotation(XYZ.BasisZ, 60 * Math.PI / 180);
//
//                     b = RotatedBoundingBox(b, rotation);
//
//                     Util.InfoMsg(string.Format(
//                         "Bounding box rotated by 60 degrees "
//                         + "extends from {0} to {1}.",
//                         Util.PointString(b.Min),
//                         Util.PointString(b.Max)));
//
//                     creator.DrawPolygon(new List<XYZ>(Util.GetBottomCorners(b)));
//
//                     tx.Commit();
//                 }
//             }
//             return Result.Succeeded;
//         }
//     }
// }