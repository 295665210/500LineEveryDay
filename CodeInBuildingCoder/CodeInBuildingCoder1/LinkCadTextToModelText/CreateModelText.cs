using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CodeInBuildingCoder1.LinkCadTextToModelText;
using Teigha.DatabaseServices.Filters;

namespace CodeInBuildingCoder1
{
    [Transaction(TransactionMode.Manual)]
    class CreateModelText : IExternalCommand
    {
        private double MODEL_TEXT_SIZE = 300;
        private double modelTextDepth = 10;
        private bool? isChecked = true;
        private bool flag = true;

        private XYZ origin = default(XYZ);
        XYZ globalPoint = default(XYZ);
        XYZ location = default(XYZ);

        private List<CADTextModel> GetCADTextModels(
            UIDocument activeUIDocument, out XYZ origin, out XYZ globalPoint, out XYZ location)
        {
            Document document = activeUIDocument.Document;
            Reference reference =
                activeUIDocument.Selection
                    .PickObject(ObjectType.PointOnElement, "点击CAD文字");
            Element element = document.GetElement(reference);

            origin = (element as ImportInstance).GetTransform().Origin;
            globalPoint = reference.GlobalPoint;
            location = globalPoint - origin;

            Debug.WriteLine("DDDD=>获取dwgFile参数开始");

            string dwgFile =
                ModelPathUtils.ConvertModelPathToUserVisiblePath(
                                                                 (activeUIDocument.Document.GetElement(
                                                                   element.GetTypeId()) as CADLinkType)
                                                                 .GetExternalFileReference().GetAbsolutePath());

            Debug.WriteLine("DDDD=>获取dwgFile参数结束");
            Debug.WriteLine(dwgFile);

            Debug.WriteLine("DDDD=>获取文字开始");

            List<CADTextModel> cadtextInfo =
                new ReadCADUtilsByDistance().GetCADTextInfo3(dwgFile,
                                                             location);
            Debug.WriteLine("DDDD=>共有" + cadtextInfo.Count + "个文字。");

            return cadtextInfo;
        }

        public Result Execute(
            ExternalCommandData commandData, ref string message,
            ElementSet elements)
        {
            UIApplication uiapplication = commandData.Application;
            UIDocument activeUIDocument = uiapplication.ActiveUIDocument;
            Application app = uiapplication.Application;
            Document document = activeUIDocument.Document;
            View activeView = activeUIDocument.ActiveView;

            //1 获得cad文字的内容和位置。:CADTextModel
            List<CADTextModel> cadTextModels = new List<CADTextModel>();
            cadTextModels = GetCADTextModels(activeUIDocument, out origin, out globalPoint, out location);

            //2 判断获得的cad文字是否为空。
            if (cadTextModels.Count == 0 || cadTextModels.Equals(null))
            {
                Debug.WriteLine("DDDD=>没有从链接的CAD文件获得任何文字，退出");
                return Result.Cancelled;
            }

            #region MyRegion
            //3 根据获得的CAD文字，创建有模型文字的公制常规模型族文件。
            //
            // string familyTemplateAddress =
            //     @"C:\ProgramData\Autodesk\RVT 2018\Family Templates\Chinese\公制常规模型.rft";
            // List<Family> families = new List<Family>();
            // Document familyDocument = app.NewFamilyDocument(familyTemplateAddress);
            #endregion

            for (int i = 0; i < cadTextModels.Count; i++)
            {
                using (Transaction tran = new Transaction(document, "Creating a Text note"))
                {
                    ElementId defaultTypeId = document.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType);

                    tran.Start();
                    TextNote note = TextNote.Create(document, activeUIDocument.ActiveView.Id,
                                                    cadTextModels[i].Location, cadTextModels[i].Text,
                                                    defaultTypeId);
                    // note.AddLeader(TextNoteLeaderTypes.TNLT_STRAIGHT_L);
                    XYZ origin2 =  globalPoint - cadTextModels[i].Location;

                    SketchPlane sketchPlane = new FilteredElementCollector(document).OfClass(typeof(SketchPlane))
                        .Cast<SketchPlane>().FirstOrDefault(x => x.GetPlane().Normal.IsAlmostEqualTo(XYZ.BasisZ));

                    Line line1 = Line.CreateBound(globalPoint, globalPoint + new XYZ(0, 1, 0));
                    Line line2 = Line.CreateBound(globalPoint, globalPoint + new XYZ(1, 0, 0));
                    ModelCurve modelCurve = document.Create.NewModelCurve(line1, sketchPlane);
                    ModelCurve modelCurve2 = document.Create.NewModelCurve(line2, sketchPlane);

                    Line line3 = Line.CreateBound(cadTextModels[i].Location, cadTextModels[i].Location + new XYZ(0, 1, 0));
                    Line line4 = Line.CreateBound(cadTextModels[i].Location, cadTextModels[i].Location + new XYZ(1, 0, 0));
                    ModelCurve modelCurve3 = document.Create.NewModelCurve(line3, sketchPlane);
                    ModelCurve modelCurve4 = document.Create.NewModelCurve(line4, sketchPlane);



                    // Line line1 = Line.CreateBound(origin2, origin2 + new XYZ(0, 1, 0));
                    // Line line2 = Line.CreateBound(origin2, origin2 + new XYZ(1, 0, 0));
                    // ModelCurve modelCurve = document.Create.NewModelCurve(line1, sketchPlane);
                    // ModelCurve modelCurve2 = document.Create.NewModelCurve(line2, sketchPlane);
                    //
                    ElementTransformUtils.RotateElement(document, note.Id,
                                                        Line.CreateBound(cadTextModels[i].Location,
                                                                         cadTextModels[i].Location + new XYZ(0, 0.0, 1)),
                                                        cadTextModels[i].Angel);

                    tran.Commit();
               
                }

                #region MyRegion
                //创建族文件

                // using (Transaction tx = new Transaction(document))
                // {
                //     tx.Start("创建族文件");
                //
                //     ModelTextType modelTextType = new FilteredElementCollector(familyDocument)
                //         .OfClass(typeof(ModelTextType)).Cast<ModelTextType>().FirstOrDefault();
                //
                //     Transaction tx2 = new Transaction(familyDocument);
                //     tx2.Start("改族内的模型文字参数");
                //     modelTextType.get_Parameter(BuiltInParameter.MODEL_TEXT_SIZE).Set(MODEL_TEXT_SIZE.MMToFeet());
                //
                //     SketchPlane sketchPlane = new FilteredElementCollector(familyDocument).OfClass(typeof(SketchPlane))
                //         .Cast<SketchPlane>().FirstOrDefault(x => x.GetPlane().Normal.IsAlmostEqualTo(XYZ.BasisZ));
                //
                //     familyDocument.FamilyCreate.NewModelText(cadTextModels[i].Text,
                //                                              modelTextType, sketchPlane,
                //                                              XYZ.Zero, HorizontalAlign.Left, modelTextDepth.MMToFeet());
                //
                //     tx2.Commit();
                //
                //     tx.Commit();
                //
                //     // //加载族文件
                // // families[i] = document.LoadFamily(familyDocument);
                // families.Add(familyDocument.LoadFamily(document));
                //
                // //修改族名
                // families.Last().Name = cadTextModels[i].Text;
                //
                // using (Transaction tx = new Transaction(document))
                // {
                //     tx.Start();
                //
                //     for (int i = 0; i < families.Count; i++)
                //     {
                //         families[i].Name = cadTextModels[i].Text;
                //         FamilySymbol familySymbol =
                //             document.GetElement(families[i].GetFamilySymbolIds().FirstOrDefault()) as FamilySymbol;
                //     }
                //     tx.Commit();
                // }

                // //分别创建族实例
                // using (Transaction tx = new Transaction(document))
                // {
                //     tx.Start("创建族实例");
                //     foreach (Family family in families)
                //     {
                //         FamilySymbol familySymbol =
                //             document.GetElement(family.GetFamilySymbolIds().FirstOrDefault()) as FamilySymbol;
                //         //判断激活
                //         if (!familySymbol.IsActive)
                //         {
                //             familySymbol.Activate();
                //         }
                //         document.Create
                //             .NewFamilyInstance(globalPoint + XYZ.BasisY.Normalize() * MODEL_TEXT_SIZE.MMToFeet(),
                //                                familySymbol, StructuralType.NonStructural);
                //     }
                //     tx.Commit();
                // }
                // } 
                #endregion
            }
            return Result.Succeeded;
        }
    }
}