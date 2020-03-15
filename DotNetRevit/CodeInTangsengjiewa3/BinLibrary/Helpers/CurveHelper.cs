using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace CodeInTangsengjiewa3.BinLibrary.Helpers
{
    public static class CurveHelper
    {
        /// <summary>
        /// 根据一点和一条曲线,求得点到曲线连线相垂直的切点
        /// 未完成的方法.
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="po"></param>
        /// <returns></returns>
        public static List<XYZ> GetPerpendicularPointList(Curve curve, XYZ po)
        {
            var result = default(List<XYZ>);
            //
            return result;
        }
    }
}
