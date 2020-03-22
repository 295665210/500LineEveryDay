// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
// using System.Windows;
// using Autodesk.Revit.Attributes;
// using Autodesk.Revit.DB;
// using Autodesk.Revit.UI;
// using Autodesk.Revit.UI.Events;
//
// namespace CodeInTangsengjiewa4.TeacherXuExercise
// {
//     /// <summary>
//     /// 
//     /// </summary>
//     [Transaction(TransactionMode.Manual)]
//     [Regeneration(RegenerationOption.Manual)]
//     [Journaling(JournalingMode.UsingCommandData)]
//     class Date0320CategoryType3 : IExternalCommand
//     {
//         public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
//         {
//             var uiapp = commandData.Application;
//             var app = uiapp.Application;
//             var uidoc = uiapp.ActiveUIDocument;
//             var doc = uidoc.Document;
//             string info = "";
//
//             var categoryFilters = new List<ElementFilter>();
//             foreach (BuiltInCategory categoryId in Enum.GetValues(typeof(BuiltInCategory)))
//             {
//                 if (categoryId !=BuiltInCategory.INVALID )
//                 {
//                     var category = Category.GetCategory(doc, categoryId); //
//                     info += category+ "\n";
//                 }
//                
//                
//                 
//                 // if ( category.CategoryType == CategoryType.Model)
//                 // {
//                 //     var categoryFilter = new ElementCategoryFilter(category.Id);
//                 //     categoryFilters.Add(categoryFilter);
//                 // }
//             }
//             TaskDialog.Show("tips", info);
//
//             // var logicalOrFilter =new LogicalOrFilter(categoryFilters);
//             // var collector = new FilteredElementCollector(doc).WherePasses(logicalOrFilter);
//             // MessageBox.Show(collector.ToList().Count.ToString());
//             return Result.Succeeded;
//         }
//     }
// }