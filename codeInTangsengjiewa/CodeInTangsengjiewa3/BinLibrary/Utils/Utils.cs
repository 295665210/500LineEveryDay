using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CodeInTangsengjiewa3
{
    class Utils
    {
        /// <summary>
        /// Gets solid objects of specified element.
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="opt"></param>
        /// <returns></returns>
        public static List<Solid> GetElementSolids(Element elem, Options opt = null)
        {
            if (null == elem)
            {
                return null;
            }
            if (null == opt)
            {
                opt = new Options();
            }
            List<Solid> solids = new List<Solid>();
            GeometryElement gElem = null;
            try
            {
                gElem = elem.get_Geometry(opt);
                IEnumerator<GeometryObject> gIter = gElem.GetEnumerator();
                gIter.Reset();
                while (gIter.MoveNext())
                {
                    solids.AddRange(GetSolids(gIter.Current));
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
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
                IEnumerator<GeometryObject> gIter2 =
            }
           
        }

        /// <summary>
        /// Gets one vertical vector of given vector,the return vector is not normalized.
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        private static XYZ GetVertVec(XYZ vec)
        {
            XYZ ret = new XYZ(-vec.Y + vec.Z, vec.X + vec.Z, -vec.Y - vec.X);
            return ret;
        }

        public static bool CanMakeBound(XYZ end0, XYZ end1)
        {
            try
            {
                Line line = Line.CreateBound(end0, end1);
                return (null != line);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}