using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Linq;

namespace CodeInTangsengjiewa4.CodeOfQian
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_NewType : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            FilteredElementCollector collector = new FilteredElementCollector(doc);
            var symbols = collector.OfClass(typeof(FamilySymbol)).Where(m => m.Name.Contains("475 x 610mm")).ToList();
            var targetSymbol = symbols.First() as FamilySymbol;
            Transaction ts = new Transaction(doc, "复制类型");
            ts.Start();
            for (int i = 100; i < 1000; i += 100)
            {
                var newSymbolName = $"HW{i} x {i}";
                if (!symbols.Select(m => m.Name).Contains(newSymbolName))
                {
                    var newType = targetSymbol.Duplicate(newSymbolName);

                    newType.LookupParameter("深度").Set(i / 304.8);
                    newType.LookupParameter("偏移基准").Set(0 / 304.8);
                    newType.LookupParameter("偏移顶部").Set(0 / 304.8);
                    newType.LookupParameter("宽度").Set(i / 304.8);
                    TaskDialog.Show("tip", "新类型创建完成");
                }
                else
                {
                    TaskDialog.Show("tip", "已经存在,不创建!");
                }
            }
            ts.Commit();
            return Result.Succeeded;
        }
    }
}