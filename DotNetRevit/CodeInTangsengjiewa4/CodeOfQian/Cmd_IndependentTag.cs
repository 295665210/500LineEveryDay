using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace CodeInTangsengjiewa4.CodeOfQian
{
    /// <summary>
    /// 过滤一张视图上的independentTag, 条件是tag标记的元素名称包含"标准"两个字
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class Cmd_IndependentTag : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;

            List<Element> collector = new FilteredElementCollector(doc).WhereElementIsNotElementType()
                .OfClass(typeof(IndependentTag)).Where(m => m.OwnerViewId == doc.ActiveView.Id)
                .Select(m => (m as IndependentTag).GetTaggedLocalElement()).Where(m => m.Name.Regex("标准")).ToList();
            List<ElementId> collectedIds = collector.Select(m => m.Id).ToList();
            string info = "";
            foreach (Element element in collector)
            {
                info += element.Id + "\n";
            }
            info += "总共有 " + collector.Count + "个元素符合要求";
            MessageBox.Show(info);
            sel.SetElementIds(collectedIds);
            return Result.Succeeded;
        }
    }

    public static class TemUtils
    {
        public static bool Regex(this string input, string pattern)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(input, pattern);
        }
    }
}