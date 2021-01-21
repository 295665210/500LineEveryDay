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
        private double aaa = 300;
        private double aaa2 = 10;
        private bool? isChecked = true;
        private bool flag = true;

        public Result Execute(
            ExternalCommandData commandData, ref string message,
            ElementSet elements)
        {
            UIApplication application = commandData.Application;
            UIDocument activeUIDocument = application.ActiveUIDocument;
            Application application2 = application.Application;
            Document document = activeUIDocument.Document;
            View activeView = activeUIDocument.ActiveView;

            try
            {
                string text2 =
                    "C:\\ProgramData\\Autodesk\\RVT 2018\\Family Templates\\Chinese\\公制常规模型.rft";
                if (text2 == null)
                {
                    TaskDialog.Show("警告", "请检查“公制结构柱”族样板路径是否正确");
                    return Result.Cancelled;
                }
                Debug.WriteLine("DDDD=>公制结构柱”族样板路径  OK");

                Reference reference =
                    activeUIDocument.Selection
                        .PickObject(ObjectType.PointOnElement, "点击CAD文字");
                Element element = document.GetElement(reference);

                XYZ origin = (element as ImportInstance).GetTransform().Origin;
                XYZ globalPoint = reference.GlobalPoint;
                XYZ location = globalPoint - origin;

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

                for (int i = 0; i < cadtextInfo.Count; i++)
                {
                    string text = cadtextInfo[i].Text;

                    Debug.WriteLine("DDDD=> " + text);

                    Document document2 = application2.NewFamilyDocument(text2);
                    Transaction transaction =
                        new Transaction(document2, "模型文字");
                    transaction.Start();
                    ModelTextType modelTextType =
                        new FilteredElementCollector(document2)
                            .OfClass(typeof(ModelTextType))
                            .Cast<ModelTextType>().FirstOrDefault();
                    modelTextType
                        .get_Parameter(BuiltInParameter.MODEL_TEXT_SIZE)
                        .Set(aaa.MMToFeet());
                    FilteredElementCollector filteredElementCollector =
                        new FilteredElementCollector(document2);
                    SketchPlane sketchPlane =
                        (from SketchPlane x in
                             filteredElementCollector
                                 .OfClass(typeof(SketchPlane))
                         where x.GetPlane().Normal.IsAlmostEqualTo(XYZ.BasisZ)
                         select x).FirstOrDefault<SketchPlane>();
                    SketchPlane SketchPlane2 =
                        (from SketchPlane x in
                             filteredElementCollector
                                 .OfClass(typeof(SketchPlane))
                         where x.GetPlane().Normal.IsAlmostEqualTo(-XYZ.BasisY)
                         select x).FirstOrDefault<SketchPlane>();

                    if (flag)
                    {
                        Debug.WriteLine("开始创建多行文字");
                        document2.FamilyCreate.NewModelText(text, modelTextType,
                         sketchPlane, XYZ.Zero, HorizontalAlign.Left,
                         aaa2.MMToFeet());
                        Debug.WriteLine("成功创建多行文字");
                    }
                    // else
                    // {
                    //     document2.FamilyCreate.NewModelText(text, modelTextType,
                    //      SketchPlane2, XYZ.Zero, HorizontalAlign.Left,
                    //      aaa2.MMToFeet());
                    // }

                    transaction.Commit();

                    Debug.WriteLine("创建模型文字族成功");

                    Family family = document2.LoadFamily(document);
                    FilteredElementCollector filteredElementCollector2 =
                        new FilteredElementCollector(document);

                    filteredElementCollector2 =
                        (FilteredElementCollector)
                        (from FamilySymbol x in filteredElementCollector2
                             .OfCategory(BuiltInCategory.OST_GenericModel)
                             .WhereElementIsNotElementType()
                         where x.Family.Name.Contains(text)
                         select x);

                    int num = filteredElementCollector2.Count<Element>();
                    Transaction transaction2 =
                        new Transaction(document, "修改族名");
                    transaction2.Start();
                    family.Name = text + num.ToString();
                    FamilySymbol familySymbol =
                        document.GetElement(family.GetFamilySymbolIds()
                                                .FirstOrDefault()) as
                            FamilySymbol;
                    if (!familySymbol.IsActive)
                    {
                        familySymbol.Activate();
                    }

                    transaction2.Commit();

                    Transaction transaction3 = new Transaction(document, "放量");
                    transaction3.Start();
                    document.Create
                        .NewFamilyInstance(globalPoint + XYZ.BasisY.Normalize() * aaa.MMToFeet() * (double) i,
                                           familySymbol,
                                           StructuralType.NonStructural);
                    transaction3.Commit();
                }
            }

            catch (Exception ex)
            {
                if (ex.Message == "Invalid Dwg Version")
                {
                    TaskDialog.Show("错误", "链接的CAD图纸版本太高，只支持2010版本以下的CAD图纸。");
                    return Result.Cancelled;
                }
                if (ex.Message.Contains("Can't open file"))
                {
                    TaskDialog.Show("错误", "CAD文件被打开，请先关闭后再进行翻模。");
                }
            }
            return Result.Succeeded;
        }
    }
}