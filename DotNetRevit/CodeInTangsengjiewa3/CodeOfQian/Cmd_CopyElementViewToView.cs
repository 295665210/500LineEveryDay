// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using System.Runtime.CompilerServices;
// using System.Text;
// using System.Threading.Tasks;
// using System.Windows;
// using System.Xml;
// using Autodesk.Revit.Attributes;
// using Autodesk.Revit.DB;
// using Autodesk.Revit.DB.Visual;
// using Autodesk.Revit.UI;
// using Autodesk.Revit.UI.Selection;
// using CodeInTangsengjiewa2.BinLibrary.Extensions;
// using CodeInTangsengjiewa2.BinLibrary.Helpers;
// using CodeInTangsengjiewa2.通用.UIs;
//
//
//*************************没搞出来***************

// namespace CodeInTangsengjiewa3.CodeOfQian
// {
//     [Transaction(TransactionMode.Manual)]
//     [Regeneration(RegenerationOption.Manual)]
//     [Journaling(JournalingMode.UsingCommandData)]
//     public class Cmd_Now_CopyElementViewToView : IExternalCommand
//     {
//         /// <summary>
//         /// what can i do with revit api now?
//         /// CopyElement:  二维视图见复制元素
//         /// </summary>
//         /// <param name="commandData"></param>
//         /// <param name="message"></param>
//         /// <param name="elements"></param>
//         /// <returns></returns>
//         public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
//         {
//             var uiapp = commandData.Application;
//             UIDocument uidoc = commandData.Application.ActiveUIDocument;
//             Document doc = uidoc.Document;
//             Selection sel = uidoc.Selection;
//
//             doc.Invoke(m =>
//                        {
//                            View view1 = doc.TCollector<View>()
//                                .Where(n => (n.get_Parameter(BuiltInParameter.ELEM_FAMILY_PARAM).AsValueString() =="楼层平面"))
//                                .FirstOrDefault(j => j.Name == "标高1");
//
//                            View view2 = doc.TCollector<View>()
//                                .Where(n => (n.get_Parameter(BuiltInParameter.ELEM_FAMILY_PARAM).AsValueString() =="楼层平面"))
//                                .FirstOrDefault(k => k.Name == "标高2");
//
//                           
//                            ICollection<ElementId> elementsTopy = sel.GetElementIds();
//
//                            Transform transform = ElementTransformUtils.GetTransformFromViewToView(view1, view1);
//
//                            var eles = ElementTransformUtils.CopyElements(view1, elementsTopy, view1, transform,);
//                        }
//                      , "复制元素2: 从视图到视图");
//
//             return Result.Succeeded;
//         }
//     }
// }

