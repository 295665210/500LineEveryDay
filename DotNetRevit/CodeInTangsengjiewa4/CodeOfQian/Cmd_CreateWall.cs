using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CodeInTangsengjiewa4.BinLibrary.Helpers;

namespace CodeInTangsengjiewa4.CodeOfQian
{
    /// <summary>
    /// create wall
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_CreateWall : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;

            IList<Curve> curves = new List<Curve>();
            curves.Add(Line.CreateBound(new XYZ(0, 0, 0), new XYZ(0, 0, 20)));
            curves.Add(Line.CreateBound(new XYZ(0, 0, 20), new XYZ(20, 0, 20)));
            curves.Add(Line.CreateBound(new XYZ(20, 0, 20), new XYZ(20, 0, 0)));
            curves.Add(Line.CreateBound(new XYZ(20, 0, 20), new XYZ(0, 0, 0)));

            ElementId wallTypeId = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls)
                .OfClass(typeof(WallType)).Cast<WallType>().OrderBy(x => x.Name).FirstOrDefault()?.Id;
            ElementId levelId = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Levels)
                .OfClass(typeof(Level)).Cast<Level>().OrderBy(x => x.Elevation).FirstOrDefault()?.Id;
            doc.Invoke(m => { Wall.Create(doc, curves, wallTypeId, levelId, false); }, "create wall");
            return Result.Succeeded;
        }
    }
}