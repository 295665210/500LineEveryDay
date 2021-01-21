using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CodeInBuildingCoder1.LinkCadTextToModelText
{
 public  static class Methord
    {
        public static double MMToFeet(this double aaa)
        {
            return UnitUtils.Convert(aaa, (DisplayUnitType)2, (DisplayUnitType)3);
            //  DUT_CENTIMETERS = 1,DUT_MILLIMETERS = 2,DUT_DECIMAL_FEET = 3,
        }

        public static void ShowTaskDialog(this string str)
        {
            TaskDialog.Show("快速提示", str);
        }
    }
}
