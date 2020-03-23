using System.Threading;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CodeInTangsengjiewa4.BinLibrary.Extensions;
using DoubleExtension = CodeInTangsengjiewa4.BinLibrary.Extensions.DoubleExtension;

namespace CodeInTangsengjiewa4.Test
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_RotateTest : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;

            var wall =
                sel.PickObject(ObjectType.Element, doc.GetSelectionFilter(m => m is Wall)).GetElement(doc) as Wall;
            var locationLine = ((wall.Location) as LocationCurve).Curve as Line;
            var startPoint = locationLine.StartPoint();
            var axisLine = Line.CreateBound(startPoint, XYZ.BasisZ);

            Transaction ts = new Transaction(doc, "rotate animation");
            ts.Start();
            for (int i = 0; i < 20; i++)
            {
                Thread.Sleep(1000);
                ElementTransformUtils.RotateElement(doc, wall.Id, axisLine, 45d.DegreeToRadius());
                uidoc.RefreshActiveView();
            }
            ts.Commit();
            return Result.Succeeded;
        }
    }
}