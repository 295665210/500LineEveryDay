using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CodeInTangsengjiewa3.BinLibrary.Extensions;
using CodeInTangsengjiewa3.BinLibrary.Helpers;

namespace CodeInTangsengjiewa3.CodeOfQian
{
    /// <summary>
    /// 旋转 墙
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class Cmd_RotateElement2 : IExternalCommand
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
                Element ele = sel.PickObject(ObjectType.Element, " 请选择一面 墙").GetElement(doc);
            }, "旋转墙");
        }

        bool LocationRotate(Application app, Element element)
        {
            bool rotated = false;
            if (element.Location is LocationCurve curve)
            {
                Curve line = curve.Curve;
                XYZ aa = line.GetEndPoint(0);
                XYZ cc = new XYZ(aa.X, aa.Y, aa.Z + 10);
                Line axis = Line.CreateBound(aa, cc);
                rotated = curve.Rotate(axis, Math.PI / 4);
            }
            if (element.Location is LocationPoint location)
            {
                XYZ aa = location.Point;
                XYZ cc = new XYZ(aa.X, aa.Y, aa.Z + 10);
                Line axis = Line.CreateBound(aa, cc);
                rotated = location.Rotate(axis, Math.PI / 4);
            }
            return rotated;
        }
    }
}