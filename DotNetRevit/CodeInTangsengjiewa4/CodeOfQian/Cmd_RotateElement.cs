using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CodeInTangsengjiewa4.BinLibrary.Extensions;
using CodeInTangsengjiewa4.BinLibrary.Helpers;


namespace CodeInTangsengjiewa4.CodeOfQian
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_RotateElement : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;
            Element element = sel.PickObject(ObjectType.Element, "please select a column").GetElement(doc);
            XYZ p1 = (element.Location as LocationPoint).Point;

            doc.Invoke(m =>
            {
                Line line = Line.CreateBound(p1, new XYZ(p1.X, p1.Y, p1.Z + 10));
                ElementTransformUtils.RotateElement(doc, element.Id, line, 30d.DegreeToRadius());
            }, "rotate column");
            return Result.Succeeded;
        }
    }
}