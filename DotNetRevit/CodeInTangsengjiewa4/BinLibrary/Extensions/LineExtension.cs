using Autodesk.Revit.DB;

namespace CodeInTangsengjiewa4.BinLibrary.Extensions
{
    public static class LineExtension
    {
        public static XYZ StartPoint(this Line line)
        {
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
            var lineOrigin = line.Origin;
            var lineDirection = line.Direction;
            var pointOnline = lineOrigin + lineDirection;

            var trans = Transform.Identity;
            trans.Origin = p.Origin;
            trans.BasisX = p.XVec;
            trans.BasisY = p.YVec;
            trans.BasisZ = p.Normal;

            var point1 = lineOrigin;
            var point2 = pointOnline;

            var point1Intrans = trans.Inverse.OfPoint(point1);
            var point2Intrans = trans.Inverse.OfPoint(point2);

            var point1InWorld = trans.OfPoint(point1Intrans);
            var point2InWorld = trans.OfPoint(point2Intrans);

            var newlineInPlan = Line.CreateBound(point1InWorld, point2InWorld);

            var unboundNewline = newlineInPlan.Clone() as Line;
            unboundNewline.MakeUnbound(); //修改曲线不影响原曲线的主体

            var unboundOriginalLine = line.Clone() as Line;
            unboundOriginalLine.MakeUnbound();
            return unboundOriginalLine.Intersect_cus(unboundOriginalLine);
        }

        public static XYZ Intersect_cus(this Line line1, Line line2)
        {
            var compareResult = line1.Intersect(line2, out IntersectionResultArray intersectionResultArray);
            //有两个返回值:
            //compareResult:
            //intersectionResultArray:相当于多申明了一个变量并赋值了
            if (compareResult != SetComparisonResult.Disjoint)
            {
                var result = intersectionResultArray.get_Item(0).XYZPoint;
                return result;
            }
            return null;
        }
    }
}