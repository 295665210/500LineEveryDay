using System.Linq;
using System.Windows;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CodeInTangsengjiewa4.BinLibrary.Helpers;


namespace CodeInTangsengjiewa4.CodeOfQian
{
    /// <summary>
    /// get viewFamily types
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_GetViewFamilyTypes : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;

            doc.Invoke(m =>
            {
                var collector = new FilteredElementCollector(doc).WhereElementIsElementType().OfType<ViewFamilyType>()
                    .OrderBy(x => x.FamilyName).ToList();
                string info = "";
                foreach (var ele in collector)
                {
                    info += ele.FamilyName + " : " + ele.Name + "\n";
                }
                MessageBox.Show(info);
            }, "show viewFamilyTypes");
            return Result.Succeeded;
        }
    }
}