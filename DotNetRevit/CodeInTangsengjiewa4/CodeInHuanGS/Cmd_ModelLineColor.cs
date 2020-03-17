using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Linq;
using Autodesk.Revit.ApplicationServices;

namespace CodeInTangsengjiewa4.CodeInHuanGS
{
    /// <summary>
    /// Revit 设置模型线的颜色有两种方法:
    /// 方法一 : 新建线的样式, 设置;
    /// 方法二: 替换视图中的图形,只对当前视图有效.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_ModelLineColor : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Application app = uiapp.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            View view = doc.ActiveView;

            Reference reference = uidoc.Selection.PickObject(ObjectType.Element);
            Element elem = doc.GetElement(reference);
            //方法一:
            Category tCat = doc.Settings.Categories.get_Item(BuiltInCategory.OST_Lines);
            Transaction ts = new Transaction(doc, "Trans");
            ts.Start();
            if (!tCat.SubCategories.Contains("MyLine"))
            {
                Category nCategory = doc.Settings.Categories.NewSubcategory(tCat, "MyLine");
                nCategory.LineColor = new Color(255, 0, 0);
            }
            doc.Regenerate();
            FilteredElementCollector temCollector = new FilteredElementCollector(doc);
            GraphicsStyle graphicsStyle =
                temCollector.OfClass(typeof(GraphicsStyle))
                    .First(m => (m as GraphicsStyle).GraphicsStyleCategory.Name == "MyLine") as GraphicsStyle;
            Parameter temParameter = elem.LookupParameter("线样式");
            temParameter.Set(graphicsStyle.Id);
            return Result.Succeeded;
        }
    }
}