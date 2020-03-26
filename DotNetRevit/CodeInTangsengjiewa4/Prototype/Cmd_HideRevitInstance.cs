using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CodeInTangsengjiewa4.Prototype
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_HideRevitInstance : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;
            var acview = doc.ActiveView;

            var collector = new FilteredElementCollector(doc);
            var revitLinkTypes = collector.OfClass(typeof(RevitLinkType)).ToList();

            Transaction ts = new Transaction(doc, "hide revit link");
            ts.Start();
            acview.HideElements(revitLinkTypes.Select(m => m.Id).ToList());
            ts.Commit();

            return Result.Succeeded;
        }
    }
}