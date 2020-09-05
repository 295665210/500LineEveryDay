using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.Annotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autodesk.Revit.DB;
using CodeInTangsengjiewa3.BinLibrary.Extensions;

namespace CodeInTangsengjiewa3.BinLibrary.Geometryalgorithm
{
    public class PolygonHelper
    {
        public static bool IsPointInRegion(XYZ po, List<XYZ> points, XYZ planNorm)
        {
            bool result = false;
            var angles = 0.0;
            for (int i = 0; i < points.Count; i++)
            {
                if (i < points.Count - 1)
                {
                    var curpo = points.ElementAt(i);
                    var nextpo = points.ElementAt(i + 1);
                    var line = Line.CreateBound(curpo, nextpo);
                    if (po.IsOnLine(line))
                    {
                        return true;
                    }

                    var angle = Angle(curpo, nextpo, po, planNorm);
                    angles += angle;
                }
                else if (i == points.Count - 1)
                {
                    var curpo = points.ElementAt(i);
                    var nextpo = points.ElementAt(0);
                    var line = Line.CreateBound(curpo, nextpo);
                    if (po.IsOnLine(line))
                    {
                        return true;
                    }
                    var angle = Angle(curpo, nextpo, po, planNorm);
                    angles += angle;
                }
            }
            // MessageBox.Show(angles.ToString());

            angles = Math.Abs(angles);
            if (angles.IsEqual(2 * Math.PI))
            {
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 计算∠point1_point0_point2 的角度
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point0"></param>
        /// <param name="planNorm"></param>
        /// <returns></returns>
        public static double Angle(XYZ point1, XYZ point2, XYZ point0, XYZ planNorm)
        {
            var line = Line.CreateBound(point1, point2);
            if (point0.IsOnLine(line))
            {
                throw new Exception("Sameline Exception");
            }

            var vec1 = (point1 - point0).Normalize();
            var vec2 = (point2 - point0).Normalize();
            if (vec1.IsSameDirection(vec2))
            {
                return 0;
            }
            else if (vec1.IsOppositeDirection(vec2))
            {
                return Math.PI;
            }

            var normal = default(XYZ);
            normal = vec1.CrossProduct(vec2).Normalize();
            var angle = vec1.AngleOnPlaneTo(vec2, normal);
            if (angle > Math.PI)
            {
                angle = angle - 2 * Math.PI;
            }
            return angle * (normal.DotProduct(planNorm));
        }
    }
}