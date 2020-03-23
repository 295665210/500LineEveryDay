using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CodeInTangsengjiewa.BinLibrary.Extensions;

namespace CodeInTangsengjiewa4.Test
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_FlipTee : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;

            var ids = sel.GetElementIds();
            if (ids.Count > 1 || ids.Count == 1)
            {
                return Result.Cancelled;
            }
            var id = sel.GetElementIds().First();
            var ele = id.GetElement(doc) as FamilyInstance;
            return Result.Succeeded;
        }
    }
}