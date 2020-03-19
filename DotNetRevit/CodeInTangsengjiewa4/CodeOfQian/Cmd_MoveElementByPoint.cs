using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CodeInTangsengjiewa4.BinLibrary.Extensions;
using CodeInTangsengjiewa4.BinLibrary.Helpers;

namespace CodeInTangsengjiewa4.CodeOfQian
{
    /// <summary>
    /// move element by point
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class Cmd_MoveElementByPoint : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;
            doc.Invoke(m =>
            {
                FamilyInstance column =
                    sel.PickObject(ObjectType.Element,
                                   doc.GetSelectionFilter(x => (BuiltInCategory) (x.Category.Id.IntegerValue) ==
                                                               BuiltInCategory.OST_Columns))
                        .GetElement(doc) as FamilyInstance;
                XYZ newPoint = new XYZ(0, 0, 0);
                (column.Location as LocationPoint).Point = newPoint;
            }, "move element by point");
            return Result.Succeeded;
        }
    }
}