using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace CodeInTangsengjiewa3.Test
{
    class Cmd_Updatefiles : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var dbapp = uiapp.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;

            OpenFileDialog opdg = new OpenFileDialog();
            opdg.Multiselect = true;
            opdg.Filter = "(*.rvt)|*.rvt|(*.rfa)|*.rfa";
            var showResult = opdg.ShowDialog();
            if (showResult == true)
            {
                var files = opdg.FileNames;
                foreach (string file in files)
                {
                    var temdoc = dbapp.OpenDocumentFile(file);
                    temdoc.Save();
                    temdoc.Close();
                }
            }
            else
            {
                return Result.Cancelled;
            }
            return Result.Succeeded;
        }
    }
}