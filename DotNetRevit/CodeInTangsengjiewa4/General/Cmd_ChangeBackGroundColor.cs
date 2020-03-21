using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CodeInTangsengjiewa4.BinLibrary.Extensions;

namespace CodeInTangsengjiewa4.General
{
    [Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class Cmd_ChangeBackGroundColor : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;

            ColorDialog colorDialog = new ColorDialog();
            var colorResult = colorDialog.ShowDialog();
            System.Drawing.Color targetColor;
            if (colorResult == DialogResult.OK)
            {
                targetColor = colorDialog.Color;
            }
            else
            {
                return Result.Cancelled;
            }
            uiapp.Application.BackgroundColor = targetColor.ToRvtColor();
            return Result.Succeeded;
        }
    }
}