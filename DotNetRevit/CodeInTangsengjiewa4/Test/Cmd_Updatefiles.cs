using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.Win32;

namespace CodeInTangsengjiewa4.Test
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_UpdateFiles : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var app = uiapp.Application;

            OpenFileDialog opdg = new OpenFileDialog();
            opdg.Multiselect = true;
            opdg.Filter = "(*.rvt)|*.rvt|(*.rva)|*.rfa";
            var showResult = opdg.ShowDialog();

            if (showResult == true)
            {
                var files = opdg.FileNames;
                foreach (string file in files)
                {
                    var temDoc = app.OpenDocumentFile(file);
                    temDoc.Save();
                    temDoc.Close();
                }
            }l
            else
            {
                return Result.Cancelled;
            }

            return Result.Succeeded;
        }
    }
}