using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;


namespace CodeInBuildingCoder1
{
    class Creator
    {
        private Document _doc;
        private Autodesk.Revit.Creation.Application _creapp;
        private Autodesk.Revit.Creation.Document _credoc;

        public Creator(Document doc)
        {
            _doc = doc;
            _credoc = doc.Create;
            _creapp = doc.Application.Create;
        }


        /// <summary>
        /// Determine the plane that a given curve resides in and return its normal vector.
        /// Ask the curve for its start and end points and some point in the middle.
        /// The latter can be obtained by asking the curve for its parameter range and
        /// evaluating it in the middle, or by tessellation. In case of tessellation,
        /// you could iterate through the tessellation points and use each one together
        /// with the start and end points to try and determine a valid plane.
        /// Once one is found, you can add debug assertions to ensure that the other
        /// tessellation points (if there are any more) are in the same plane.
        /// In the case of the line, the tessellation only returns two points.
        /// I once heard that that is the only element that can do that, all
        /// non-linear curves return at least three. So you could use this property
        /// to determine that a line is a line (and add an assertion as well, if you like).
        /// Update, later: please note that the Revit API provides an overload of the
        /// NewPlane method taking a CurveArray argument.
        /// </summary>
        XYZ GetCurveNormal(Curve curve)
        {
            IList<XYZ> pts = curve.Tessellate();
            int n = pts.Count;

            Debug.Assert(1 < n,
                         "expected at least two points "
                         + "from curve tessellation");

            XYZ p = pts[0];
            XYZ q = pts[n - 1];
            XYZ v = q - p;
            XYZ w, normal = null;

            if (2 == n)
            {
                Debug.Assert(curve is Line,
                             "expected non-line element to have "
                             + "more than two tessellation points");

                // For non-vertical lines, use Z axis to
                // span the plane, otherwise Y axis:

                double dxy = Math.Abs(v.X) + Math.Abs(v.Y);

                w = (dxy > Util.TolPointOnPlane)
                        ? XYZ.BasisZ
                        : XYZ.BasisY;

                normal = v.CrossProduct(w).Normalize();
            }
            else
            {
                int i = 0;
                while (++i < n - 1)
                {
                    w = pts[i] - p;
                    normal = v.CrossProduct(w);
                    if (!normal.IsZeroLength())
                    {
                        normal = normal.Normalize();
                        break;
                    }
                }

#if DEBUG
                {
                    XYZ normal2;
                    while (++i < n - 1)
                    {
                        w = pts[i] - p;
                        normal2 = v.CrossProduct(w);
                        Debug.Assert(normal2.IsZeroLength()
                                     || Util.IsZero(normal2.AngleTo(normal)),
                                     "expected all points of curve to "
                                     + "lie in same plane");
                    }
                }
#endif // DEBUG
            }
            return normal;
        }
        
        private SketchPlane NewSketchPlaneContainCurve(Curve curve)
        {
            XYZ p = curve.GetEndPoint(0);
            XYZ normal = GetCurveNormal(curve);
            Plane plane = Plane.CreateByNormalAndOrigin(normal, p);

#if DEBUG
            if (!(curve is Line))
            {
                //CurveArray a = _creapp.NewCurveArray();
                //a.Append( curve );
                //Plane plane2 = _creapp.NewPlane( a ); // 2016

                List<Curve> a = new List<Curve>(1);
                a.Add(curve);
                CurveLoop b = CurveLoop.Create(a);
                Plane plane2 = b.GetPlane(); // 2017

                Debug.Assert(Util.IsParallel(plane2.Normal,
                                             plane.Normal), "expected equal planes");

                Debug.Assert(Util.IsZero(plane2.SignedDistanceTo(
                                                                 plane.Origin)), "expected equal planes");
            }
#endif // DEBUG

            //return _credoc.NewSketchPlane( plane ); // 2013

            return SketchPlane.Create(_doc, plane); // 2014
        }

        public ModelCurve CreateModelCurve(Curve curve)
        {
            return _credoc.NewModelCurve(curve, NewSketchPlaneContainCurve(curve));
        }
    }
}