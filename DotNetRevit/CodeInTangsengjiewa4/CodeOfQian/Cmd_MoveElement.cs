using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CodeInTangsengjiewa4.BinLibrary.Helpers;
using CodeInTangsengjiewa4.BinLibrary.Extensions;

namespace CodeInTangsengjiewa4.CodeOfQian
{
    /// <summary>
    /// move element
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class Cmd_MoveElement : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;
            ElementId elementId = sel.PickObject(ObjectType.Element, doc.GetSelectionFilter(m => m is Wall)).ElementId;
            doc.Invoke(m => { ElementTransformUtils.MoveElement(doc, elementId, new XYZ(1000d.MmToFeet(), 1000d.MmToFeet(), 0)); },
                       "move element");
            return Result.Succeeded;
        }
    }
}