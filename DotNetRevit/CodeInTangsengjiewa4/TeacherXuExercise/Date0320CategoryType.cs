// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using Autodesk.Revit.Attributes;
// using Autodesk.Revit.DB;
// using Autodesk.Revit.UI;
//
// namespace CodeInTangsengjiewa4.TeacherXuExercise
// {
//     /// <summary>
//     ///2020年3月20日布置的作业：把文档中属于模型类别  CategoryType.Model 的元素 都过滤出来
//     /// 没搞出来
//     /// </summary>
//     [Transaction(TransactionMode.Manual)]
//     [Regeneration(RegenerationOption.Manual)]
//     [Journaling(JournalingMode.UsingCommandData)]
//     class Date0320CategoryType : IExternalCommand
//     {
//         public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
//         {
//             var uiapp = commandData.Application;
//             var app = uiapp.Application;
//             var uidoc = uiapp.ActiveUIDocument;
//             var doc = uidoc.Document;
//             var sel = uidoc.Selection;
//
//             //找到所有id大于1的元素
//             BuiltInParameter testPara = BuiltInParameter.ID_PARAM;
//             //提供者
//             ParameterValueProvider pvp = new ParameterValueProvider(new ElementId((int) testPara));
//
//             //评估者
//             FilterNumericRuleEvaluator fnrv = new FilterNumericGreater();
//
//             //规则者
//             ElementId ruleValId = new ElementId(1); //Id大于1, 由于文件里的元素太多， 运行后能按一年确定不带停的。
//
//             //创建规则过滤器和对应的元素过滤器
//             FilterRule fRule = new FilterElementIdRule(pvp, fnrv, ruleValId);
//             ElementParameterFilter filter = new ElementParameterFilter(fRule);
//             FilteredElementCollector collector = new FilteredElementCollector(doc);
//
//             var list = collector.WherePasses(filter).ToElements();
//             var list2 = list.First().Category.CategoryType.ToString();
//
//
//             // var list2 = list.Where(m => typeof(m.Category) == CategoryType.Model);
//
//             string info = "";
//             info += "共有" + list2 + "个元素.\n";
//
//             // foreach (Element element in list3)
//             // {
//             //     info += element.Id + " : " + element.Name + ".\n";
//             // }
//             TaskDialog.Show("tips", info);
//             return Result.Succeeded;
//         }
//     }
// }