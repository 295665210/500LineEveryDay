using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CodeInTangsengjiewa3.BinLibrary.Helpers;

namespace CodeInTangsengjiewa3.CodeOfQian
{
    /// <summary>
    /// get viewFamilyTypes
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class Cmd_GetViewFamilyTypes : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var app = uiapp.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;

            doc.Invoke(m =>
            {
                var collector = new FilteredElementCollector(doc).WhereElementIsElementType().OfType<ViewFamilyType>()
                    .OrderBy(j => j.FamilyName).ToList();
                string info = "";
                foreach (var ele in collector)
                {
                    info += ele.FamilyName + " : " + ele.Name + "\n";
                }
                MessageBox.Show(info);
            }, "Show viewFamilyTypes");
            return Result.Succeeded;
        }
    }
}