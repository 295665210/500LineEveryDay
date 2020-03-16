using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace CodeInTangsengjiewa4.BinLibrary.Utils
{
    public class Utils
    {
        public static List<Solid> GetElementSolids(Element element, Options options = null)
        {
            if (null == element)
            {
                return null;
            }
            if (null == options)
            {
                options = new Options();
            }
            List<Solid> solids = new List<Solid>();
            GeometryElement gElem = null;
            try
            {
                gElem = element.get_Geometry(options);
                IEnumerator<GeometryObject> gIter = gElem.GetEnumerator();
                gIter.Reset();
                while (gIter.MoveNext())
                {
                    solids.AddRange(GetSolids(gIter.Current));
                }
            }
            catch (Exception e)
            {
                string error = e.ToString();
            }
            return solids;
        }

        public static List<Solid> GetSolids(GeometryObject gObj)
        {
            List<Solid> solids = new List<Solid>();
            if (gObj is Solid)
            {
                Solid solid = gObj as Solid;
                if (solid.Faces.Size > 0 && solid.Volume > 0)
                {
                    solids.Add(gObj as Solid);
                }
            }
            else if (gObj is GeometryInstance)
            {
                IEnumerator<GeometryObject> gIter2 = (gObj as GeometryInstance).GetInstanceGeometry().GetEnumerator();
                gIter2.Reset();
                while (gIter2.MoveNext())
                {
                    solids.AddRange(GetSolids(gIter2.Current));
                }
            }
            else if (gObj is GeometryElement)
            {
                IEnumerator<GeometryObject> gIter2 = (gObj as GeometryElement).GetEnumerator();
                gIter2.Reset();
                while (gIter2.MoveNext())
                {
                    solids.AddRange(GetSolids(gIter2.Current));
                }
            }
            return solids;
        }

        /// <summary>
        /// draws model curves by given geometry curves
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="curves"></param>
        /// <param name="reviseTrf"></param>
        /// <returns></returns>
        public static List<ElementId> DrawModelCurve(Document doc, List<Curve> curves, Transform reviseTrf = null)
        {
            List<ElementId> newIds = new List<ElementId>();
            foreach (Curve curve in curves)
            {
                if (curve.IsBound)
                {
                    Curve reviseCurve = (null != reviseTrf && !reviseTrf.IsIdentity)
                                            ? curve.CreateTransformed(reviseTrf)
                                            : curve;
                    newIds.Add(CreateModelCurve(doc, reviseCurve));
                }
            }
            return newIds;
        }

        private static ElementId CreateModelCurve(Document doc, Curve curve, SketchPlane sp = null)
        {
            Line line = curve as Line;
            Arc arc = curve as Arc;
            Ellipse ellipse = curve as Ellipse;
            //样条: 埃尔米特 Hermite
            HermiteSpline spline = curve as HermiteSpline;
            //非统一有理样条
            NurbSpline nbSpline = curve as NurbSpline;
            if (line != null && null == sp)
            {
                XYZ normal = GetVertVec(line.Direction).Normalize();
                XYZ origin = line.GetEndPoint(0);
                sp = SketchPlane.Create(doc, Plane.CreateByNormalAndOrigin(normal, origin));
            }
            else if (arc != null && null == sp)
            {
                XYZ normal = arc.Normal;
                sp = SketchPlane.Create(doc, Plane.CreateByNormalAndOrigin(normal, arc.Center));
            }
            else if (nbSpline != null && null == sp)
            {
                Transform tran = nbSpline.ComputeDerivatives(0, false);
                XYZ normal = GetVertVec(tran.BasisX).Normalize();
                XYZ origin = spline.GetEndPoint(0);
                sp = SketchPlane.Create(doc, Plane.CreateByNormalAndOrigin(normal, origin));
            }
            if (sp == null)
            {
                throw new ArgumentException("not valid sketchplane to create curve" + curve.GetType().Name);
            }
            ModelCurve mCurve = doc.Create.NewModelCurve(curve, sp);
            return (null != mCurve) ? mCurve.Id : ElementId.InvalidElementId;
        }

        private static XYZ GetVertVec(XYZ vec)
        {
            XYZ ret = new XYZ(-vec.Y + vec.Z, vec.X + vec.Z, -vec.Y - vec.X);
            return ret;
        }

        public static bool CanMakeBound(XYZ end0, XYZ end1)
        {
            try
            {
                Line ln = Line.CreateBound(end0, end1);
                return (null != ln);
            }
            catch (System.Exception)
            {
                return false;
            }
        }
    }
}