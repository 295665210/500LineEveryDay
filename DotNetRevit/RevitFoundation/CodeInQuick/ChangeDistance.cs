// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
// using Autodesk.Revit.Attributes;
// using Autodesk.Revit.DB;
// using Autodesk.Revit.UI;
// using Autodesk.Revit.UI.Selection;
// using RevitFoundation.CodeInQuick.Source.UpPipe;
//
//
// namespace RevitFoundation.CodeInQuick
// {
//     [Transaction(TransactionMode.Manual)]
//     internal class ChangeDistance : IExternalCommand
//     {
//         public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
//         {
//             Result result;
//             try
//             {
//                 UIApplication uiapp = commandData.Application;
//                 var app = uiapp.Application;
//                 Document doc = uiapp.ActiveUIDocument.Document;
//                 UIDocument uidoc = uiapp.ActiveUIDocument;
//                 View view = uidoc.ActiveView;
//                 TaskDialog.Show("第一步", "请选择需要调整的喷头", TaskDialogCommonButtons.Ok);
//
//                 Selection selection = uidoc.Selection;
//
//                 IList<Reference> list = selection.PickObjects(ObjectType.Element,
//                     new ChangeDistance.pentouselectionfilter(), "选择喷头");
//                 if (list.Count == 0)
//                 {
//                     TaskDialog.Show("tips", "没有被选中的喷头");
//                     result = Result.Cancelled;
//                 }
//                 else
//                 {
//                     string a = null;
//                     string text = this.ShowDialog(a);
//                     if (text == null)
//                     {
//                         result = Result.Cancelled;
//                     }
//                     else
//                     {
//                         double num = Convert.ToDouble(text);
//                         double num2 = UnitUtils.Convert(num, DisplayUnitType.DUT_MILLIMETERS,
//                             DisplayUnitType.DUT_DECIMAL_FEET);
//                         TaskDialog.Show("第二步", "请选择链接文件的目标楼板", TaskDialogCommonButtons.Ok);
//
//                         Reference reference = uidoc.Selection.PickObject(ObjectType.LinkedElement, "请选择链接的楼板");
//
//                         RevitLinkInstance revitLinkInstance = doc.GetElement(reference) as RevitLinkInstance;
//                         Document linkDocument = revitLinkInstance.GetLinkDocument();
//                         Floor floor = linkDocument.GetElement(reference.LinkedElementId) as Floor;
//
//                         Face face = ChangeDistance.FindWallFace(floor);
//
//                         foreach (Reference reference2 in list)
//                         {
//                             FamilyInstance familyInstance = doc.GetElement(reference2) as FamilyInstance;
//                             if (familyInstance.Category.Name == "Sprinklers" || familyInstance.Category.Name == "喷头")
//                             {
//                                 Transform transform = familyInstance.GetTransform();
//                                 XYZ origin = transform.Origin;
//                                 IntersectionResult intersectionResult = face.Project(origin);
//                                 if (intersectionResult != null)
//                                 {
//                                     double distance = intersectionResult.Distance;
//                                     XYZ xyz = new XYZ(0.0, 0.0, distance - num2);
//                                     using (Transaction ts = new Transaction(doc, "修改喷头高度"))
//                                     {
//                                         ts.Start();
//                                         FailureHandlingOptions failureHandlingOptions = ts.GetFailureHandlingOptions();
//                                         ChangeDistance.MyPreProcessor failuresPreprocessor = new MyPreProcessor();
//                                         failureHandlingOptions.SetFailuresPreprocessor(failuresPreprocessor);
//                                         ts.SetFailureHandlingOptions(failureHandlingOptions);
//                                         ElementTransformUtils.MoveElement(doc, familyInstance.Id, xyz);
//                                         ts.Commit();
//                                     }
//                                 }
//                             }
//                         }
//                     }
//                 }
//                 return result = Result.Succeeded;
//             }
//             catch (Exception e)
//             {
//                 message = e.Message;
//                 TaskDialog.Show("Error", string.Format("错误信息：\n{0}", message));
//                 result = Result.Cancelled;
//             }
//             return result;
//         }
//
//         private static Face FindWallFace(Floor floor)
//         {
//             Face result = null;
//             GeometryElement geometryElement = floor.get_Geometry(new Options
//             {
//                 ComputeReferences = true,
//                 DetailLevel = ViewDetailLevel.Medium
//             });
//             foreach (GeometryObject geometryObject in geometryElement)
//             {
//                 Solid solid = geometryObject as Solid;
//                 if (!(solid != null) || solid.Faces.Size <= 0)
//                 {
//                 }
//                 foreach (object obj in solid.Faces)
//                 {
//                     Face face = (Face) obj;
//                     PlanarFace planarFace = face as PlanarFace;
//                     if (planarFace != null && planarFace.FaceNormal.AngleTo(new XYZ(0.0, 0.0, -1.0)) < 0.02)
//                     {
//                         result = face;
//                     }
//                 }
//             }
//             return result;
//         }
//
//         public string ShowDialog(string a)
//         {
//             string text = null;
//             UpPipeWpf upPipeWpf = new UpPipeWpf();
//             bool? flag = upPipeWpf.ShowDialog();
//             string result;
//             if (flag.GetValueOrDefault() & flag != null)
//             {
//                 text = upPipeWpf.TextboxHight.Text;
//                 result = text;
//             }
//             else
//             {
//                 result = text;
//             }
//             return result;
//         }
//
//         public class MyPreProcessor : IFailuresPreprocessor
//         {
//             public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
//             {
//                 string transactionName = failuresAccessor.GetTransactionName();
//
//                 IList<FailureMessageAccessor> failureMessages = failuresAccessor.GetFailureMessages();
//                 FailureProcessingResult result;
//                 if (failureMessages.Count == 0)
//                 {
//                     result = FailureProcessingResult.Continue;
//                 }
//                 else if (transactionName.Equals("创建楼板"))
//                 {
//                     foreach (FailureMessageAccessor failureMessageAccessor in failureMessages)
//                     {
//                         failuresAccessor.DeleteWarning(failureMessageAccessor);
//                     }
//                     result = FailureProcessingResult.ProceedWithCommit;
//                 }
//                 else
//                 {
//                     result = FailureProcessingResult.Continue;
//                 }
//                 return result;
//             }
//         }
//
//
//         private class pentouselectionfilter : ISelectionFilter
//         {
//             public bool AllowElement(Element elem)
//             {
//                 return elem is FamilyInstance;
//             }
//
//             public bool AllowReference(Reference reference, XYZ position)
//             {
//                 return false;
//             }
//         }
//     }
// }