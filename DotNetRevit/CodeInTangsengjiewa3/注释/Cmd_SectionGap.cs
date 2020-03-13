﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CodeInTangsengjiewa3.BinLibrary.Extensions;
using UIFrameworkServices;

namespace CodeInTangsengjiewa3.注释
{
    /// <summary>
    /// 模仿revit操作,打断剖面线
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_SectionGap : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;

            var eleId = sel.PickObject(ObjectType.Element, doc.GetSelectionFilter(m =>
            {
                return m.Category.Id ==
                       new ElementId(BuiltInCategory
                                         .OST_Views);
            })).ElementId;
            string commandId = "ID_SECTION_GAP";
            sel.SetElementIds(new List<ElementId>() {eleId});
            CommandHandlerService.invokeCommandHandler(commandId);
            return Result.Succeeded;
        }
    }
}