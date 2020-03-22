using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CodeInTangsengjiewa4.TeacherXuExercise
{
    /// <summary>
    ///2020年3月20日布置的作业：把文档中属于模型类别  CategoryType.Model 的元素 都过滤出来
    /// 
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Date0320CategoryType : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var app = uiapp.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;

            FilteredElementCollector collector = new FilteredElementCollector(doc);

            var list = collector.WhereElementIsNotElementType()
                .Where(m => m.Category != null && m.Category.CategoryType == CategoryType.Model).ToList();//document里是存在category是null的元素,要先排除,否则报错

            var list2= collector.WhereElementIsElementType()
                .Where(m => m.Category != null && m.Category.CategoryType == CategoryType.Model).ToList();
            var list3 = list.Union(list2).ToList();

            string info = "";
            info += "list共有" + list.Count()+ "个元素.\n";
            info += "list2共有" + list2.Count()+ "个元素.\n";
            info += "list3共有" + list3.Count()+ "个元素.\n";

            foreach (Element element in list3)
            {
                info += element.Id + " : " + element.Name + ".\n";
            }

            TaskDialog.Show("tips", info);
            return Result.Succeeded;
        }
    }
}