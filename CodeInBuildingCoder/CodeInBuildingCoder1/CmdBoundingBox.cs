using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CodeInBuildingCoder1
{
    [Transaction(TransactionMode.Manual)]
    class CmdBoundingBox : IExternalCommand
    {
        /// <summary> 
        /// XYZ wrapper class implementing IComparable. 
        /// </summary> 
        class XyzComparable : XYZ, IComparable<XYZ>
        {
            public XyzComparable(XYZ a) : base(a.X, a.Y, a.Z) { }

            int IComparable<XYZ>.CompareTo(XYZ a)
            {
                return Util.Compare(this, a);
            }
        }

        /// <summary>
        /// return a rotated bounding box around
        /// the origin in the XY plan.
        /// We cannot just rotate the min and max points,
        /// because the rotated max point may easily end up being "smaller" in some coordinate than the
        /// min.
        /// To work around that,we extract all four bounding box corners, rotate each of them and
        /// determine new min and max values from those.
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        private static BoundingBoxXYZ RotateBoundingBox(BoundingBoxXYZ b, Transform t)
        {
            double height = b.Max.Z - b.Min.Z;
            //four corners : lower left ,lower right ,upper right ,upper left
            XYZ[] corners = Util.GetBottomCorners(b);
            XyzComparable[] cornersTransformed =
                corners.Select<XYZ, XyzComparable>(p => new XyzComparable(t.OfPoint(p))).ToArray();
            b.Min = cornersTransformed.Min();
            b.Max = cornersTransformed.Max();
            b.Max = height * XYZ.BasisZ;

            return b;
        }


        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            Element e = Util.SelectSingleElement(uidoc, "An element");

            if (null == e)
            {
                message = "No element selected";
                return Result.Failed;
            }

            ///try to call this property returns the compile time error:
            ///Property,indexer,or event 'BoundingBox' is not supported by the
            /// language; try directly calling accessor method ''
            ///
            View view = null;
            BoundingBoxXYZ b = e.get_BoundingBox(view);
            if (null == b)
            {
                view = commandData.View;
                b = e.get_BoundingBox(view);
            }

            if (null == b)
            {
                Util.InfoMsg(Util.ElementDescription(e) + " has no bounding box");
            }

            else
            {
                using (Transaction tx = new Transaction(doc))
                {
                    tx.Start("Draw Model Line Bounding Box Outling");
                    Debug.Assert(b.Transform.IsIdentity, "expected identity bounding box transform");

                    string in_view = (null == view) ? "model space" : "view" + view.Name;

                    Util.InfoMsg(string.Format("Element bounding box of {0} in " + "{1} extends from {2} to {3} .",
                                               Util.ElementDescription(e), in_view, Util.PointString(b.Min),
                                               Util.PointString(b.Max)));
                    Creator creator = new Creator(doc);
                    creator.DrawPolygon(new List<XYZ>(Util.GetBottomCorners(b)));

                    Transform rotation = Transform.CreateRotation(XYZ.BasisZ,   2*Math.PI / 180);

                    b = RotateBoundingBox(b, rotation);

                    Util.InfoMsg(string.Format(
                                               "Bounding box rotated by 60 degrees"
                                               + "extends from {0} to {1} .",
                                               Util.PointString(b.Min),
                                               Util.PointString(b.Max)));

                    creator.DrawPolygon(new List<XYZ>(Util.GetBottomCorners(b)));

                    tx.Commit();
                }
            }
            return Result.Succeeded;
        }
    }
}