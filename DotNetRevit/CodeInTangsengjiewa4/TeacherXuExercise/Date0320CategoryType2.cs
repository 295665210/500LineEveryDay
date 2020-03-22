using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CodeInTangsengjiewa4.TeacherXuExercise
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Date0320CategoryType2 : IExternalCommand
    {
        /// <summary>
        /// 将多个过滤器作为逻辑或过滤器
        /// </summary>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var app = uiapp.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;

            var categories = doc.Settings.Categories;
            var categoryFilters = new List<ElementFilter>();
       

            foreach (Category category in categories)
            {
                var categoryType = category.CategoryType;
                if (categoryType == CategoryType.Model)
                {
                    var categoryFilter = new ElementCategoryFilter(category.Id);
                    categoryFilters.Add(categoryFilter);
                }
            }

            var logicalOrFilter = new LogicalOrFilter(categoryFilters);
            var collector = new FilteredElementCollector(doc);
            var targetCollector = collector.WherePasses(logicalOrFilter).ToList();
            MessageBox.Show(targetCollector.Count.ToString());
            return Result.Succeeded;
        }
    }
}