using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace CodeInTangsengjiewa3.BinLibrary.Extensions
{
    public static class LineExtension
    {
        public static XYZ StartPoint(this Line line)
        {
            // line.IsBound 描述曲线是否在一个区间内
            if (line.IsBound)
            {
                return line.GetEndPoint(0);
            }
            return null;
        }

        public static XYZ EndPoint(this Line line)
        {
            if (line.IsBound)
            {
                return line.GetEndPoint(1);
            }
            return null;
        }

        public static XYZ Intersect_cus(this Line line, Plane p)
        {
            var lineorigin = line.Origin;
            var linedir = line.Direction;

            var pointOnLine = lineorigin + linedir;
            //Revit 提供了Transform类来做二次开发时的坐标转换。 你可以给Transform对象进行赋值，构造一个变换矩阵。然后使用这个变化矩阵把给定的坐标点的坐标转成目标坐标系。
            //https://zhuanlan.zhihu.com/p/99697122

            var trans = Transform.Identity;
            trans.Origin = p.Origin;
            trans.BasisX = p.XVec;
            trans.BasisY = p.YVec;
            trans.BasisZ = p.Normal;

            var point1 = lineorigin;
            var point2 = pointOnLine;

            var point1Intrans = trans.Inverse.OfPoint(point1);
            var point2Intrans = trans.Inverse.OfPoint(point2);

            point1Intrans = new XYZ(point1Intrans.X, point1Intrans.Y, 0);
            point2Intrans = new XYZ(point2Intrans.X, point2Intrans.Y, 0);

            var point1InWorld = trans.OfPoint(point1Intrans);
            var point2InWorld = trans.OfPoint(point2Intrans);

            var newLineInPlan = Line.CreateBound(point1InWorld, point2InWorld);

            var unboundLine = newLineInPlan.Clone() as Line;
            unboundLine.MakeUnbound(); //如果曲线被标记为只读(因为它是直接从Revit元素或集合/聚合对象提取的)，那么调用此方法将导致该对象被更改为携带原始曲线的断开连接的副本。修改不会影响原始曲线或提供它的对象。

            var unboundOriginalLine = line.Clone() as Line;
            unboundOriginalLine.MakeUnbound();

            return unboundLine.Intersect_cus(unboundOriginalLine);
        }

        public static XYZ Intersect_cus(this Line line1, Line line2)
        {
            var compareResult = line1.Intersect(line2, out IntersectionResultArray intersectResult);
            if (compareResult != SetComparisonResult.Disjoint)
            {
                var result = intersectResult.get_Item(0).XYZPoint;
                return result;
            }
            return null;
        }
    }
}