using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using UIFrameworkServices;

namespace CodeInTangsengjiewa4.Senior
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_CallSystemCmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            var sel = uidoc.Selection;

            var ele = sel.PickObject(ObjectType.Element, "please select a section symbols");
            sel.SetElementIds(new List<ElementId>() {ele.ElementId});
            CommandHandlerService.invokeCommandHandler("ID_SECTION_GAP");
            sel.SetElementIds(new List<ElementId>()); //清空当前选择的内容
            return Result.Succeeded;
        }
    }
}