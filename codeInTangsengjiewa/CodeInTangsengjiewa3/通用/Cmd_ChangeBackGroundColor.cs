using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CodeInTangsengjiewa3.BinLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeInTangsengjiewa3.通用
{
    /// <summary>
    /// 更改视图背景颜色
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_ChangeBackGroundColor : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
           var uiapp =commandData.Application;
            var uidoc =uiapp.ActiveUIDocument;
            var doc =uidoc.Document;
            var sel =uidoc.Selection;

            ColorDialog  colorDialog =new ColorDialog();
            var colorResult =colorDialog.ShowDialog();
            var targetColor = default(System.Drawing.Color);
            if (colorResult == DialogResult.OK)
            {
                targetColor = colorDialog.Color;
            }

            else
            {
                return Result.Cancelled;
            }

            uiapp.Application.BackgroundColor =targetColor.ToRvtColor();

            return Result.Succeeded;
        }
    }
}
