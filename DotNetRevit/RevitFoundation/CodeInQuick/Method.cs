using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitFoundation.CodeInQuick
{
    public static class Method
    {
        public static bool CheckString2(this string becheck)
        {
            return Method.CheckStringIsDouble(becheck) && !becheck.Contains("-");
        }

        public static bool CheckStringIsDouble(string beCheck)
        {
            double num;
            return double.TryParse(beCheck, out num);
        }
    }
}