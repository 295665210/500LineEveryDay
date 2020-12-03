using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInTangsengjiewa3.通用
{
    [Transaction(TransactionMode.Manual)]
    class Cmd_CategoryFilter : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp =commandData.Application;
            var uidoc=uiapp.ActiveUIDocument;
            var doc =uidoc.Document;
            var sel =uidoc.Selection;

            //方法一
            var categories =doc.Settings.Categories;
            var categoryFilters = new List<ElementFilter>();
            foreach (Category category in categories)
            {
                var categoryType =category.CategoryType;
                if (categoryType ==CategoryType.Model)
                {
                    var categoryfilter = new ElementCategoryFilter(category.Id);
                    categoryFilters.Add(categoryfilter);
                }
            }

            var logicalFilter =new LogicalOrFilter(categoryFilters);
            var collector = new FilteredElementCollector(doc);

            var targetCollector =collector.WherePasses(logicalFilter);


            //方法二
            foreach (BuiltInCategory builtInid in Enum.GetValues(typeof(BuiltInCategory))   )  
            {
                var cate = Category.GetCategory(doc, builtInid);
                var categoryType =cate.CategoryType;
                if (categoryType == CategoryType.Model)
                {
                    ///
                }
            }

            return Result.Succeeded;

        }
    }
}
