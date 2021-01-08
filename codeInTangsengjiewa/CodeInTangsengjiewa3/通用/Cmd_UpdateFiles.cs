using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Microsoft.Win32;

namespace CodeInTangsengjiewa3.通用
{
    /// <summary>
    /// 升级文件
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_UpdateFiles : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            View acView = uidoc.ActiveView;

            var filePath = default(string);

            OpenFileDialog opfd = new OpenFileDialog();
            opfd.Filter = "(*.rfa)|*.rfa|(*.rvt)|*.rvt";

            opfd.Multiselect = true;
            opfd.FileOk += OnfileOk;
            var dialogresult = opfd.ShowDialog();

            var count = opfd.FileNames.Length;
            string[] files = new string[count];

            if (dialogresult == true)
            {
                files = opfd.FileNames;
            }

            foreach (var file in files)
            {
                var temdoc = uiapp.Application.OpenDocumentFile(file);
                temdoc.Save();
                temdoc.Close();
            }

            return Result.Succeeded;
        }

        private void OnfileOk(object sender, CancelEventArgs e)
        {
            (sender as OpenFileDialog).RestoreDirectory = true;
        }
    }
}