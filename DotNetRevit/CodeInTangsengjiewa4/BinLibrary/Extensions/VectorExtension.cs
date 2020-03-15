using Autodesk.Revit.DB;
using System;

namespace CodeInTangsengjiewa4.BinLibrary.Extensions
{
    public static class VectorExtension
    {
        private static double precision = 1e-6;

        public static bool IsParallel(this XYZ vector1, XYZ vector2)
        {
            return vector1.IsSameDirection(vector2) || vector1.IsOppositeDirection(vector2);
        }

        /// <summary>
        /// 判断同向
        /// </summary>
        /// <param name="dir1"></param>
        /// <param name="dir2"></param>
        /// <returns></returns>
        public static bool IsSameDirection(this XYZ dir1, XYZ dir2)
        {
            bool result = false;
            double dotProduct = dir1.Normalize().DotProduct(dir2.Normalize());
            //dotproduct :点乘, a向量点乘b向量= a的模 * b的模 * cos(zeta);
            if (Math.Abs(dotProduct - 1) < precision)
            {
                result = true;
            }
            return result;
        }

        public static bool IsOppositeDirection(this XYZ dir1, XYZ dir2)
        {
            bool result = false;
            double dotProduct = dir1.Normalize().DotProduct(dir2.Normalize());
            if (Math.Abs(dotProduct + 1) < precision)
            {
                return true;
            }
            return result;
        }
    }
}