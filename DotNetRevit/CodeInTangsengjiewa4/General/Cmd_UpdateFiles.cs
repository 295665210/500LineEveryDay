using System.ComponentModel;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.Win32;

namespace CodeInTangsengjiewa4.General
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_UpdateFiles : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Application app = commandData.Application.Application;

            OpenFileDialog opdg = new OpenFileDialog();
            opdg.Filter = "(*.rfa)|*.rfa|(*.rvt)|*.rvt";
            opdg.Multiselect = true;
            opdg.FileOk += OnFileOk;
            var dialogResult = opdg.ShowDialog();
            var count = opdg.FileNames.Length;
            string[] files = new string[count];
            if (dialogResult == true)
            {
                files = opdg.FileNames;
            }
            foreach (var file in files)
            {
                var temDoc = app.OpenDocumentFile(file);
                temDoc.Save();
                temDoc.Close();
            }
            return Result.Succeeded;
        }

        private void OnFileOk(object sender, CancelEventArgs e)
        {
            (sender as OpenFileDialog).RestoreDirectory = true;
        }
    }
}