using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace CodeInTangsengjiewa3.BinLibrary.Extensions
{
    public static class MepCurveExtension
    {
        public static Line LocationLine(this MEPCurve mep)
        {
            Line result = null;
            result = (mep.Location as LocationCurve).Curve as Line;
            return result;
        }
    }
}