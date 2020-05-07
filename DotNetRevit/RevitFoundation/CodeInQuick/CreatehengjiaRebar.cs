using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CodeInTangsengjiewa2.BinLibrary.Extensions;

namespace RevitFoundation.CodeInQuick
{
    [Transaction(TransactionMode.Manual)]
    internal class CreatehengjiaRebar : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var app = uiapp.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;

            CreatehengjiaRebarWPF createhengjiaRebarWpf = new CreatehengjiaRebarWPF();
            createhengjiaRebarWpf.ShowDialog();
            Result result;
            if (!createhengjiaRebarWpf.Iscontinue)
            {
                result = Result.Cancelled;
            }
            else
            {
                try
                {
                    Reference reference = uidoc.Selection.PickObject(ObjectType.Element, "1");
                    XYZ globalPoint = reference.GlobalPoint;
                    Element element = doc.GetElement(reference);
                    RebarBarType rebarBarType1 =
                        new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rebar)
                            .OfClass(typeof(RebarBarType)).WhereElementIsElementType()
                            .FirstOrDefault((Element m) => m.Name == "6 HPB300") as RebarBarType;

                    RebarBarType rebarBarType2 = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rebar)
                        .OfClass(typeof(RebarBarType)).WhereElementIsElementType()
                        .FirstOrDefault((Element m) => m.Name == "8 HPB300") as RebarBarType;

                    RebarBarType rebarBarType3 = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rebar)
                        .OfClass(typeof(RebarBarType)).WhereElementIsElementType()
                        .FirstOrDefault((Element m) => m.Name == "10 HPB300") as RebarBarType;

                    if (rebarBarType1 == null || rebarBarType2 == null || rebarBarType3 == null)
                    {
                        TaskDialog.Show("tips", "项目中没有钢筋类型：6 HPB300 / 8 HRB400 / 10 HRB400 ");
                        return Result.Cancelled;
                    }

                    XYZ xyz = globalPoint;
                    XYZ xyz2 = xyz + 71.0d.MmToFeet() * XYZ.BasisX;
                    IList<Curve> list = new List<Curve>();
                    IList<Curve> list2 = new List<Curve>();
                    IList<Curve> list3 = new List<Curve>();
                    IList<Curve> list4 = new List<Curve>();
                    IList<Curve> list5 = new List<Curve>();
                    IList<Curve> list6 = new List<Curve>();
                    IList<Curve> list7 = new List<Curve>();

                    IList<ElementId> list8 = new List<ElementId>();

                    List<XYZ> list9 = new List<XYZ>();
                    List<XYZ> list10 = new List<XYZ>();
                    List<XYZ> list11 = new List<XYZ>();
                    List<XYZ> list12 = new List<XYZ>();

                    Line item = Line.CreateBound(globalPoint, globalPoint + 10.0 * XYZ.BasisX);

                    Line item2 = Line.CreateBound(globalPoint, globalPoint + 10.0 * XYZ.BasisX);

                    Line item3 = Line.CreateBound(globalPoint, globalPoint + 10.0 * XYZ.BasisX);

                    double num = Math.Floor(Convert.ToDouble(createhengjiaRebarWpf.totalLength.Text) / 100.0);

                    string Type = createhengjiaRebarWpf.m_type.Text;

                    this.typeParam = this.GetTypeParam(Type);

                    this.typeH = this.GetTypeH(Type);

                    this.TopRebarR = this.GetTopRebarR(Type);

                    this.TopRebarBarType = (new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rebar)
                        .OfClass(typeof(RebarBarType)).WhereElementIsElementType()
                        .FirstOrDefault((Element m) => m.Name == this.GetTopRebarBarType(Type)) as RebarBarType);

                    this.m_腹杆角度angel = this.Get腹杆角度angel(Type);

                    bool? isChecked = createhengjiaRebarWpf.vertical.IsChecked;
                    this.isVerti = (isChecked.GetValueOrDefault() & isChecked != null);
                    Line line = Line.CreateBound(globalPoint, globalPoint + 100 * XYZ.BasisX);
                    Line line2 = Line.CreateBound(globalPoint, globalPoint + 100.0 * XYZ.BasisX);

                    double num2 = (num * 100.0).MmToFeet();
                    double num3 = 0.0;
                    bool flag = true;
                    bool flag2 = true;
                    if (this.isVerti)
                    {
                        int num4 = 0;
                        while ((double) num4 <= num + 1.0)
                        {
                            if (num3 < 2750.0)
                            {
                                if (num4 == 0)
                                {
                                    XYZ xyz3 = new XYZ(xyz.X + 4.0.MmToFeet(), xyz.Y, xyz.Z);
                                }
                            }
                        }
                    }
                }

                catch (Exception)
                {
                }
                result = Result.Succeeded;
            }
            return result;
        }

        private double GetTypeParam(string type)
        {
            double result;
            if (type != "A80")
            {
                if (type != "A90")
                {
                    if (type != "A100")
                    {
                        if (type != "B80")
                        {
                            if (type != "B90")
                            {
                                if (type != "B100")
                                {
                                    result = 89.44.MmToFeet();
                                }
                                else
                                {
                                    result = 110.0.MmToFeet();
                                }
                            }
                            else
                            {
                                result = 100.0.MmToFeet();
                            }
                        }
                        else
                        {
                            result = 89.44.MmToFeet();
                        }
                    }
                    else
                    {
                        result = 110.0.MmToFeet();
                    }
                }

                else
                {
                    result = 100d.MmToFeet();
                }
            }
            else
            {
                result = 89.44d.MmToFeet();
            }

            return result;
        }

        private double GetTypeH(string type)
        {
            double result;
            if (!(type == "A80"))
            {
                if (!(type == "A90"))
                {
                    if (!(type == "A100"))
                    {
                        if (!(type == "B80"))
                        {
                            if (!(type == "B90"))
                            {
                                if (!(type == "B100"))
                                {
                                    result = 80.0;
                                }
                                else
                                {
                                    result = 100.0;
                                }
                            }
                            else
                            {
                                result = 90.0;
                            }
                        }
                        else
                        {
                            result = 80.0;
                        }
                    }
                    else
                    {
                        result = 100.0;
                    }
                }
                else
                {
                    result = 90.0;
                }
            }
            else
            {
                result = 80.0;
            }
            return result;
        }

        private double GetTopRebarR(string type)
        {
            double result;
            if (!(type == "A80"))
            {
                if (!(type == "A90"))
                {
                    if (!(type == "A100"))
                    {
                        if (!(type == "B80"))
                        {
                            if (!(type == "B90"))
                            {
                                if (!(type == "B100"))
                                {
                                    result = 4.0;
                                }
                                else
                                {
                                    result = 5.0;
                                }
                            }
                            else
                            {
                                result = 5.0;
                            }
                        }
                        else
                        {
                            result = 5.0;
                        }
                    }
                    else
                    {
                        result = 4.0;
                    }
                }
                else
                {
                    result = 4.0;
                }
            }
            else
            {
                result = 4.0;
            }
            return result;
        }

        private string GetTopRebarBarType(string type)
        {
            string result;
            if (!(type == "A80"))
            {
                if (!(type == "A90"))
                {
                    if (!(type == "A100"))
                    {
                        if (!(type == "B80"))
                        {
                            if (!(type == "B90"))
                            {
                                if (!(type == "B100"))
                                {
                                    result = "8 HRB400";
                                }
                                else
                                {
                                    result = "10 HRB400";
                                }
                            }
                            else
                            {
                                result = "10 HRB400";
                            }
                        }
                        else
                        {
                            result = "10 HRB400";
                        }
                    }
                    else
                    {
                        result = "8 HRB400";
                    }
                }
                else
                {
                    result = "8 HRB400";
                }
            }
            else
            {
                result = "8 HRB400";
            }
            return result;
        }

        private double Get腹杆角度angel(string type)
        {
            double result;
            if (!(type == "A80"))
            {
                if (!(type == "A90"))
                {
                    if (!(type == "A100"))
                    {
                        if (!(type == "B80"))
                        {
                            if (!(type == "B90"))
                            {
                                if (!(type == "B100"))
                                {
                                    result = 73.0;
                                }
                                else
                                {
                                    result = 76.5;
                                }
                            }
                            else
                            {
                                result = 75.5;
                            }
                        }
                        else
                        {
                            result = 73.5;
                        }
                    }
                    else
                    {
                        result = 76.0;
                    }
                }
                else
                {
                    result = 75.0;
                }
            }
            else
            {
                result = 73.0;
            }
            return result;
        }
        //就没有更好的方法来搞了吗？？

        private bool isVerti = true;
        private double key = 1.0;
        private double typeH = 80.0;

        private double typeParam = 89.44.MmToFeet();


        private double TopRebarR = 8.0;

        private double m_腹杆角度angel = 73.0;

        private RebarBarType TopRebarBarType;

        private class MyClass : IFailuresPreprocessor
        {
            public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
            {
                string transactionName = failuresAccessor.GetTransactionName();

                IList<FailureMessageAccessor> failureMessages = failuresAccessor.GetFailureMessages();

                FailureProcessingResult result;

                if (failureMessages.Count == 0)
                {
                    result = FailureProcessingResult.ProceedWithCommit;
                }
                else
                {
                    result = FailureProcessingResult.Continue;
                }
                return result;
            }
        }
    }
}