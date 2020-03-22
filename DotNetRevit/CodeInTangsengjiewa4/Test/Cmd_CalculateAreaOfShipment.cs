using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CodeInTangsengjiewa4.BinLibrary.Extensions;

namespace CodeInTangsengjiewa4.Test
{
    /// <summary>
    /// 统计墙的内外表面积
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class Cmd_CalculateAreaOfShipment : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var app = uiapp.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;

            var wall =
                sel.PickObject(ObjectType.Element, doc.GetSelectionFilter(m => m is Wall)).GetElement(doc) as Wall;
            var facesOutRef = HostObjectUtils.GetSideFaces(wall, ShellLayerType.Exterior);
            var facesInRef = HostObjectUtils.GetSideFaces(wall, ShellLayerType.Interior);
            var faceOut = wall.GetGeometryObjectFromReference(facesOutRef.First()) as Face;
            var faceIn = wall.GetGeometryObjectFromReference(facesInRef.First()) as Face;

            var area = default(double);
            area += faceOut.Area;
            area += faceIn.Area;
            area = UnitUtils.ConvertFromInternalUnits(area, DisplayUnitType.DUT_CANDELAS_PER_SQUARE_METER);
            area = Math.Round(area, 3);
            MessageBox.Show(area.ToString() + "m^2");
            return Result.Succeeded;
        }
    }
}