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
using CodeInTangsengjiewa3.BinLibrary.Extensions;
using CodeInTangsengjiewa3.BinLibrary.Helpers;

namespace CodeInTangsengjiewa3.CodeOfQian
{
    /// <summary>
    /// create window(familyInstance)
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class Cmd_CreateWindow : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var app = uiapp.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;
            var view = doc.ActiveView;

            var symbol = new FilteredElementCollector(doc).WhereElementIsElementType()
                .OfCategory(BuiltInCategory.OST_Windows).OfClass(typeof(FamilySymbol)).Cast<FamilySymbol>()
                .FirstOrDefault();
            TaskDialog.Show("tips", symbol.Name);
            Reference reference = sel.PickObject(ObjectType.Element, doc.GetSelectionFilter(m => m is Wall));
            TaskDialog.Show("tps", reference.GetElement(doc).Name);
            Line line = (reference.GetElement(doc).Location as LocationCurve).Curve as Line;
            XYZ startPoint = line.GetEndPoint(0);
            XYZ endPoint = line.GetEndPoint(1);
            XYZ midPoint = (startPoint + endPoint) / 2;
            Line position = Line.CreateBound(startPoint, midPoint);
            doc.Invoke(m =>
            {
                if (!symbol.IsActive)
                {
                    symbol.Activate();
                }
                doc.Create.NewFamilyInstance(midPoint, symbol, reference.GetElement(doc), StructuralType.NonStructural);
                //ctrl + p : show parameter info(resharper shortcut)
            }, "create wall");

            return Result.Succeeded;
        }
    }
}