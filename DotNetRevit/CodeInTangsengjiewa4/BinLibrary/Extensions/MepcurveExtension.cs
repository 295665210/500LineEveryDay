using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace CodeInTangsengjiewa4.BinLibrary.Extensions
{
    public static class MepCurveExtension
    {
        public static Line LocationLine(this MEPCurve mepCurve)
        {
            Line result = null;
            result = (mepCurve.Location as LocationCurve).Curve as Line;
            return result;
        }
    }
}