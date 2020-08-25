using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace CodeInTangsengjiewa3.BinLibrary.Extensions
{
    public static class VectorExtension
    {
        /// <summary>
        /// 定义误差
        /// </summary>
        private static double precision = 0.000001;
        /// <summary>
        /// 判断平行
        /// </summary>
        /// <param name="vector1"></param>
        /// <param name="vector2"></param>
        /// <returns></returns>
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
            if (Math.Abs(dotProduct - 1) < precision)
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// 判断反向
        /// </summary>
        /// <param name="dir1"></param>
        /// <param name="dir2"></param>
        /// <returns></returns>
        public static bool IsOppositeDirection(this XYZ dir1, XYZ dir2)
        {
            bool result = false;
            double dotProduct = dir1.Normalize().DotProduct(dir2.Normalize());
            if (Math.Abs(dotProduct + 1) < precision)
            {
                result = true;
            }
            return result;
        }
    }
}