using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitFoundation.CodeInQuick
{
    [Transaction(TransactionMode.Manual)]
    internal class CleanSectionView : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Application app = uiapp.Application;
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            View actView = uidoc.ActiveView;
            FilteredElementCollector filteredElementCollector = new FilteredElementCollector(doc);
            IList<Element> list =
                (from x in filteredElementCollector.OfClass(typeof(ViewSection)).OfCategory(BuiltInCategory.OST_Views)
                 where x.Name.Contains("剖面") && !x.Name.Contains("视图")
                 select x).ToList<Element>();

            Transaction ts = new Transaction(doc, "清理剖面视图");
            ts.Start();
            foreach (Element element in list)
            {
                View view = element as View;
                if (view.CanBePrinted)
                {
                    FilteredElementCollector filteredElementCollector2 = new FilteredElementCollector(doc, element.Id);
                    IList<Element> source = filteredElementCollector2.OfCategory(BuiltInCategory.OST_Dimensions)
                        .ToElements();
                    if (source.Count() < 1 && element.Id != actView.Id)
                    {
                        doc.Delete(element.Id);
                    }
                }
            }
            ts.Commit();
            return Result.Succeeded;
        }
    }
}