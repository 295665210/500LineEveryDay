using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace CodeInTangsengjiewa4.CodeOfQian
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_EnumViewFamily : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            string info = "";
            //枚举转为数字
            info += "枚举转数字int : \n ";
            info += (int) ViewFamily.Elevation;

            //数字转为枚举
            info += "数字转为枚举 : \n";
            info += ((ViewFamily) 114).ToString();

            //枚举转为字符串
            info += "枚举转为字符串 : \n";
            info += ViewFamily.Elevation.ToString();
            info += ViewFamily.Elevation;

            //将字符串转为枚举
            info += "字符串转为枚举 : \n";
            info += (ViewFamily) Enum.Parse(typeof(ViewFamily), "Elevation");

            info += @"Enum.Format : " + "\n";
            info += Enum.Format(typeof(ViewFamily), 114, "g");

            info += Enum.GetUnderlyingType(typeof(ViewFamily));

            info += Enum.GetValues(typeof(ViewFamily));
            foreach (var item in Enum.GetNames(typeof(ViewFamily)))
            {
                info += item + "\n";
            }
            foreach (var value in Enum.GetValues(typeof(ViewFamily)))
            {
                info += (int) value + "\n";
            }

            TaskDialog.Show("tips", info);
            return Result.Succeeded;
        }
    }
}