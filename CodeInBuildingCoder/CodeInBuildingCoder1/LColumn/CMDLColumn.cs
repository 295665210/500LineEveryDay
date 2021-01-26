using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace CodeInBuildingCoder1.LColumn
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class CMDLColumn : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Application app = uiapp.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            string text = @"C:\ProgramData\Autodesk\RVT 2018\Family Templates\Chinese\公制结构柱.rft";
            if (!System.IO.Directory.Exists(text))
            {
                TaskDialog.Show("警告", "插件安装目录没有找到族样板文件：公制结构柱.rft。");
                return Result.Cancelled;
            }

            if (new TaskDialog("异形柱")
                        {Title = "异形柱翻模", MainInstruction = "请点击cad轮廓线", CommonButtons = TaskDialogCommonButtons.Ok}
                    .Show() != TaskDialogResult.Ok)
            {
                return Result.Cancelled;
            }

            Result result;
            try
            {
                Reference reference = uidoc.Selection.PickObject(ObjectType.PointOnElement, "请选择柱cad轮廓");
                Element element = doc.GetElement(reference);
                GeometryObject geometryObjectFromReference = element.GetGeometryObjectFromReference(reference);
                Transform transform = (element as ImportInstance).GetTransform();
                PolyLine polyLine = geometryObjectFromReference as PolyLine;
                CurveArray curveArray = new CurveArray();
                CurveArrArray curveArrArray = new CurveArrArray();
                IList<XYZ> coordinates = polyLine.GetCoordinates();

                for (int i = 0; i < coordinates.Count - 1; i++)
                {
                    if (i < coordinates.Count - 2)
                    {
                        Line line = Line.CreateBound(transform.OfPoint(coordinates[i]),
                                                     transform.OfPoint(coordinates[i + 1]));
                        curveArray.Append(line);
                    }
                    else
                    {
                        Line line2 = Line.CreateBound(transform.OfPoint(coordinates[i]),
                                                      transform.OfPoint(coordinates[0]));
                    }
                }
                curveArrArray.Append(curveArray);

                Document document2 = app.NewFamilyDocument(text);
                using (Transaction transaction = new Transaction(document2, "Create Family"))
                {
                    transaction.Start();
                    FamilyManager familyManager = document2.FamilyManager;
                    FamilyParameter familyParameter =
                        familyManager.AddParameter("材质", BuiltInParameterGroup.PG_MATERIALS, ParameterType.Material,
                                                   false);
                    CurveArrArray curveArrArray2 = curveArrArray;
                    SketchPlane sketchPlane = GetSketchPlane(document2);
                    Extrusion extrusion = document2.FamilyCreate.NewExtrusion(true, curveArrArray2, sketchPlane, 13.12);
                    document2.Regenerate();
                    extrusion.Location.Move(new XYZ(-transform.OfPoint(coordinates[0]).X,
                                                    -transform.OfPoint(coordinates[0]).Y, 0.0));
                    Reference reference2 = null;
                    Options options = new Options()
                    {
                        ComputeReferences = true,
                        DetailLevel = ViewDetailLevel.Fine
                    };
                    foreach (GeometryObject geometryObject in extrusion.get_Geometry(options))
                    {
                        if (geometryObject is Solid)
                        {
                            foreach (object obj in (geometryObject as Solid).Faces)
                            {
                                Face face = (Face) obj;
                                if (face.ComputeNormal(new UV()).IsAlmostEqualTo(XYZ.BasisZ))
                                {
                                    reference2 = face.Reference;
                                }
                            }
                        }
                    }

                    View view = GetView(document2);
                    Reference topLevel = GetTopLevel(document2);
                    document2.FamilyCreate.NewAlignment(view, topLevel, reference2).IsLocked = true;
                    document2.Regenerate();
                    Parameter parameter = extrusion.get_Parameter(BuiltInParameter.MATERIAL_ID_PARAM);
                    familyManager
                        .AssociateElementParameterToFamilyParameter(parameter,
                                                                    familyParameter); //定义一个材质参数，将族参数里的材质，与自定义的材质参数关联。
                    transaction.Commit();
                }

                Family family = document2.LoadFamily(doc);
                //.LoadFamily
                // Loads the contents of this family document into another document.
                // 参数:targetDocument: he target document where the family will be loaded.
                document2.Close(false);
                FilteredElementCollector filteredElementCollector = new FilteredElementCollector(doc);
                filteredElementCollector.OfCategory(BuiltInCategory.OST_StructuralColumns)
                    .WhereElementIsElementType();
                List<Element> list = new List<Element>();
                foreach (Element item in filteredElementCollector)
                {
                    list.Add(item);
                }
                list.Count<Element>();
                int num = filteredElementCollector.Count<Element>() - 10;
                string str = "L shape";
                string text2 = "C:\\ProgramData\\Autodesk\\Revit\\Addins\\2016\\icon\\TCtools.xml";
                if (File.Exists(text2))
                {
                    str = ReadNodeFromXML(text2, "异形柱命名")[0];
                }
                string text3 = str + num.ToString();
                FamilySymbol familySymbol = null;
                using (Transaction transaction2 = new Transaction(doc, "ActiveFamilySymbol"))
                {
                    transaction2.Start();
                    family.Name = text3;
                    FilteredElementCollector filteredElementCollector2 = new FilteredElementCollector(doc);
                    filteredElementCollector2.OfClass(typeof(FamilySymbol));
                    foreach (Element element2 in filteredElementCollector2)
                    {
                        FamilySymbol familySymbol2 = (FamilySymbol) element2;
                        if (familySymbol2 != null && familySymbol2.Family.Name == text3)
                        {
                            familySymbol = familySymbol2;
                            familySymbol.Name = "Standard";
                        }
                    }
                    if (!familySymbol.IsActive)
                    {
                        familySymbol.Activate();
                    }
                    transaction2.Commit();
                }
                Level genLevel = doc.ActiveView.GenLevel;

                using (Transaction transaction3 = new Transaction(doc, "Create Instance"))
                {
                    transaction3.Start();
                    doc.Create.NewFamilyInstance(transform.OfPoint(coordinates[0]), familySymbol, genLevel,
                                                 StructuralType.Column);
                    transaction3.Commit();
                }
            }

            catch (Exception e)
            {
                message = e.Message;
                return Result.Cancelled;
            }
            return Result.Succeeded;
        }


        private static Reference GetTopLevel(Document document2)
        {
            FilteredElementCollector filteredElementCollector = new FilteredElementCollector(document2);
            var level = filteredElementCollector.OfClass(typeof(Level)).First(m => m.Name == "高于参考标高") as Level;
            return new Reference(level);
        }

        private static View GetView(Document document2)
        {
            FilteredElementCollector filteredElementCollector = new FilteredElementCollector(document2);
            return filteredElementCollector.OfClass(typeof(View)).First(x => x.Name == "前") as View;
        }

        public static SketchPlane GetSketchPlane(Document doc)
        {
            FilteredElementCollector filteredElementCollector = new FilteredElementCollector(doc);
            return filteredElementCollector
                           .OfClass(typeof(SketchPlane))
                           .First(m => m.Name == "低于参照标高")
                       as SketchPlane;
        }

        public static List<string> ReadNodeFromXML(string filePath, string nodeName)
        {
            List<string> list = new List<string>();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(filePath);
            foreach (object obj in xmlDocument.GetElementsByTagName(nodeName))
            {
                XmlNode xmlNode = (XmlNode) obj;
                list.Add(xmlNode.InnerText);
            }
            return list;
        }
    }
}