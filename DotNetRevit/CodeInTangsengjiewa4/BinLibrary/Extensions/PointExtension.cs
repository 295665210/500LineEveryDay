using Autodesk.Revit.DB;
using System;
using CodeInTangsengjiewa.BinLibrary.Extensions;

namespace CodeInTangsengjiewa4.BinLibrary.Extensions
{
    public static class PointExtension
    {
        private static double precision = 1e-6;

        /// <summary>
        /// 将指定点投影到曲线上
        /// </summary>
        /// <param name="point"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public static XYZ ProjectToXLine(this XYZ point, Line line)
        {
            Line l1 = line.Clone() as Line;
            if (l1.IsBound)
            {
                l1.MakeUnbound();
            }
            return l1.Project(point).XYZPoint;
        }

        public static bool IsEqual(double d1, double d2)
        {
            double diff = Math.Abs(d1 - d2);
            return diff < precision;
        }

        /// <summary>
        /// 判断点是否在直线上
        /// </summary>
        /// <param name="point"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public static bool IsOnLine(this XYZ point, Line line)
        {
            XYZ end1 = line.GetEndPoint(0);
            XYZ end2 = line.GetEndPoint(1);

            XYZ vecPToEnd1 = end1 - point;
            XYZ vecPToEnd2 = end2 - point;
            if (point.DistanceTo(end1) < precision || point.DistanceTo(end2) < precision)
            {
                return true;
            }
            if (vecPToEnd1.IsOppositeDirection(vecPToEnd2))
            {
                return true;
            }
            return false;
        }

        public static bool IsOnXLine(this XYZ po, Line line)
        {
            var line1 = line.Clone() as Line;
            line1.MakeUnbound();
            if (po.DistanceTo(line1) < precision)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 求点到细线的距离; 系统自带的Line.Distance(point)遇到line有界的时候,会输出point到line的端点的长度的情况.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="xLine"></param>
        /// <returns></returns>
        public static double DistanceTo(this XYZ point1, Line xLine)
        {
            double result = double.NegativeInfinity;
            XYZ p1OnLine = point1.ProjectToXLine(xLine);
            result = point1.DistanceTo(p1OnLine);
            return result;
        }
    }
}