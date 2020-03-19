using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CodeInTangsengjiewa4.BinLibrary.Extensions;
using CodeInTangsengjiewa4.BinLibrary.Helpers;

namespace CodeInTangsengjiewa4.CodeOfQian
{
    /// <summary>
    /// 旋转墙
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class Cmd_RotateElement2 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;
            doc.Invoke(m =>
            {
                Element element = sel.PickObject(ObjectType.Element, "请选择一面墙").GetElement(doc);
                LocationRotate(element);
            }, "旋转墙");
            return Result.Succeeded;
        }

        void LocationRotate(Element element)
        {
            if (element.Location is LocationCurve curve)
            {
                Curve line = curve.Curve;
                XYZ aa = line.GetEndPoint(0);
                XYZ cc = new XYZ(aa.X, aa.Y, aa.Z + 10);
                Line axis = Line.CreateBound(aa, cc);
                curve.Rotate(axis, 45d.DegreeToRadius());
            }
            if (element.Location is LocationPoint locationPoint)
            {
                XYZ aa = locationPoint.Point;
                XYZ cc = new XYZ(aa.X, aa.Y, aa.Z + 10);
                Line axis = Line.CreateBound(aa, cc);
                locationPoint.Rotate(axis, 45d.DegreeToRadius());
            }
        }
    }
}