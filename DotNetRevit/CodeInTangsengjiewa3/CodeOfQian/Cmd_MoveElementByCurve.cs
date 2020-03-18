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
    /// move element by curve
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class Cmd_MoveElementByCurve : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;

            doc.Invoke(m =>
            {
                Wall wall =
                    sel.PickObject(ObjectType.Element, doc.GetSelectionFilter(x => x is Wall)).GetElement(doc) as Wall;
                Line wallLine = Line.CreateBound(XYZ.Zero, new XYZ(2000d.MmToFeet(), -10000d.MmToFeet(), 0));
                (wall.Location as LocationCurve).Curve = wallLine;
            }, "move wall by curve");
            return Result.Succeeded;
        }
    }
}