using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CodeInTangsengjiewa3.BinLibrary.Extensions;
using CodeInTangsengjiewa3.BinLibrary.Helpers;

namespace CodeInTangsengjiewa3.CodeOfQian
{
    /// <summary>
    ///  move wall  
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

            ElementId eleId = sel.PickObject(ObjectType.Element, doc.GetSelectionFilter(m => m is Wall)).ElementId;
            doc.Invoke(m => { ElementTransformUtils.MoveElement(doc, eleId, new XYZ(1000d.MmToFeet(), 1000d.MmToFeet(), 0)); },
                       "move element");
            return Result.Succeeded;
        }
    }
}