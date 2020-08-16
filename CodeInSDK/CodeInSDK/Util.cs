using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Form = Autodesk.Revit.DB.Form;
using MessageBox = System.Windows.MessageBox;

namespace CodeInSDK
{
    class Util
    {
        public const double _eps = 1.0e-9;
        public static double EPS
        {
            get { return _eps; }
        }

        public static double MinLineLength
        {
            get
            {
                return _eps;
            }
        }

        public static double TolPointOnPlane
        {
            get
            {
                return _eps;
            }
        }

        /// <summary>
        /// Predicate to test whewther two points or 
        /// vectors can be considered equal with the 
        /// given tolerance.
        /// </summary>
        public static bool IsEqual
        (
            double p,
            double q,
            double tolerance = _eps
        )
        {
            return 0 == Compare(p, q, tolerance);
        }


        public static int Compare
        (
            double a,
            double b,
            double tolerance = _eps
        )
        {
            return IsEqual(a, b, tolerance)
                ? 0
                : (a < b ? -1 : 1);
        }

        public static int Compare(XYZ p, XYZ q, double tolerance = _eps)
        {
            int d = Compare(p.X, q.X, tolerance);
            if (0 == d)
            {
                d = Compare(p.Y, q.Y, tolerance);
                if (0 == d)
                {
                    Compare(p.Z, q.Z, tolerance);
                }
            }
            return d;
        }

        /// <summary>
        /// return the bottom four xyz corners of the given
        /// bounding box in the xy plane at the bb minimum
        /// z elevation in the order lower left , lower right,upper right, upper left:
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static XYZ[] GetBottomCorners(BoundingBoxXYZ b)
        {
            return GetBottomCorners(b, b.Min.Z);
        }

        public static XYZ[] GetBottomCorners(BoundingBoxXYZ b, double z)
        {
            return new XYZ[]
            {
                new XYZ(b.Min.X, b.Min.Y, z),
                new XYZ(b.Max.X, b.Min.Y, z),
                new XYZ(b.Max.X, b.Max.Y, z),
                new XYZ(b.Min.X, b.Max.Y, z),
            };
        }

#region Element Selection
        public static Element SelectSingleElement(UIDocument uidoc, string description)
        {
            if (ViewType.Internal == uidoc.ActiveView.ViewType)
            {
                TaskDialog.Show("Error", "Cannot pick element in thie view: " + uidoc.ActiveView.Name);
                return null;
            }

            try
            {
                Reference r = uidoc.Selection.PickObject(ObjectType.Element, "Please select" + description);
                return uidoc.Document.GetElement(r);
            }
            catch (OperationCanceledException)
            {
                return null;
            }
        }
#endregion

#region Display a message
        const string _caption = "The Building Coder";

        public static void InfoMsg(string msg)
        {
            Debug.WriteLine(msg);
            MessageBox.Show(msg, _caption, MessageBoxButton.OK, (MessageBoxImage) MessageBoxIcon.Information);
        }

        public static string ElementDescription(Element e)
        {
            if (null == e)
            {
                return "<null>";
            }

            FamilyInstance fi = e as FamilyInstance;
            string typeName = e.GetType().Name;

            string categoryName = (null == e.Category) ? string.Empty : e.Category.Name + " ";

            string familyName = (null == fi) ? string.Empty : fi.Symbol.Family.Name + " ";

            string symbolName = (null == fi || e.Name.Equals(fi.Symbol.Name)) ? string.Empty : fi.Symbol.Name + " ";

            return string.Format("{0} {1}{2}{3}<{4} {5}>", typeName, categoryName, familyName, symbolName,
                e.Id.IntegerValue, e.Name);
        }

        public static string PointString(XYZ p, bool onlySpaceSeparator = false)
        {
            string format_string = onlySpaceSeparator ? "{0} {1} {2}" : "({0},{1},{2})";

            return string.Format(format_string, RealString(p.X), RealString(p.Y), RealString(p.Z));
        }

        public static string RealString(double e)
        {
            return e.ToString("0.##");
        }
#endregion
    }
}