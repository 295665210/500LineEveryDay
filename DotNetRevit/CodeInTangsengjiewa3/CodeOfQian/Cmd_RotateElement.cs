using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CodeInTangsengjiewa3.BinLibrary.Extensions;
using CodeInTangsengjiewa3.BinLibrary.Helpers;

namespace CodeInTangsengjiewa3.CodeOfQian
{
    /// <summary>
    /// 旋转一个柱子
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class Cmd_RotateElement : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var app = uiapp.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;

            doc.Invoke(m =>
            {
                //假设是一个柱子
                Element ele = sel.PickObject(ObjectType.Element, "请选择一根柱子").GetElement(doc);
                XYZ p1 = (ele.Location as LocationPoint).Point;
                Line line = Line.CreateBound(p1, new XYZ(p1.X, p1.Y, p1.Z + 10));
                ElementTransformUtils.RotateElement(doc, ele.Id, line, 30d.DegreeToRadius());
            }, "旋转柱子");
            return Result.Succeeded;
        }
    }
}