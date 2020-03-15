using Autodesk.Revit.DB;
using CodeInTangsengjiewa4.BinLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeInTangsengjiewa4.BinLibrary.GeometryAlgorithm
{
    public class PolygonHelper
    {
        public static bool IsPointInRegion(XYZ po, List<XYZ> points, XYZ planeNorm)
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
                    var angle = Angle(curpo, nextpo, po, planeNorm);
                    angle += angle;
                }
            }
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


        public static double Angle(XYZ point1, XYZ point2, XYZ point0, XYZ planeNorm)
        {
            var line = Line.CreateBound(point1, point2);
            if (point0.IsOnLine(line))
            {
                throw new Exception("Same line exception");
            }
            var vec1 = (point1 - point0).Normalize();
            var vec2 = (point2 - point1).Normalize();
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
            return angle * (normal.DotProduct(planeNorm));
        }
    }
}