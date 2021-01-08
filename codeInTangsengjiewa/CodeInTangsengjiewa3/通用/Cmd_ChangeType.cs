 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CodeInTangsengjiewa3.BinLibrary.Extensions;

namespace CodeInTangsengjiewa3.通用
{
    /// <summary>
    /// 修改元素类型
    /// </summary>
    class Cmd_ChangeType : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;

            var acview = uidoc.ActiveView;

            var firstEle = sel.PickObject(ObjectType.Element, doc.GetSelectionFilter(m => m is Pipe)).GetElement(doc);
            var secondEle = sel.PickObject(ObjectType.Element, doc.GetSelectionFilter(m => m is Pipe)).GetElement(doc);

            var collector = new FilteredElementCollector(doc);
            var pipecollector = collector.OfClass(typeof(Pipe));

            var targets = pipecollector.Where(m => m.GetTypeId().IntegerValue == firstEle.GetTypeId().IntegerValue);

            Transaction ts = new Transaction(doc, "修改类型");
            ts.Start();
            foreach (var element in targets)
            {
                element.ChangeTypeId(secondEle.GetTypeId());
            }
            ts.Commit();
            return Result.Succeeded;
        }
    }
}