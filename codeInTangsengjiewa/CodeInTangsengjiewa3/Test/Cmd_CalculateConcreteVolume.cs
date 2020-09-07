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
using CodeInTangsengjiewa3.BinLibrary.Extensions;

namespace CodeInTangsengjiewa3.Test
{
    [Transaction(TransactionMode.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    [Regeneration(RegenerationOption.Manual)]
    class Cmd_CalculateConcreteVolume : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;

            var ele = sel.PickObject(ObjectType.Element).GetElement(doc);

            var options = new Options();
            options.DetailLevel = ViewDetailLevel.Fine;

            var geometryElement = ele.get_Geometry(options);

            double volume = getVolumes(geometryElement);

            var volumeString = Math.Round(volume, 3).ToString();

            MessageBox.Show(volumeString + "m^3");
            return Result.Succeeded;
        }

        public double getVolumes(GeometryElement geoEle)
        {
            double result = default(double);
            var geoEnu = geoEle.GetEnumerator();
            while (geoEnu.MoveNext())
            {
                var currentGeo = geoEnu.Current;
                if (currentGeo is Solid solid)
                {
                    result += solid.Volume;
                }
                else if (currentGeo is GeometryInstance geoIns)
                {
                    var temgeoele = geoIns.SymbolGeometry;
                    var geoenu1 = temgeoele.GetEnumerator();
                    while (geoenu1.MoveNext())
                    {
                        var currentgeo1 = geoenu1.Current;
                        if (currentgeo1 is Solid solid1)
                        {
                            result += solid1.Volume;
                        }
                    }
                }
            }
            result = UnitUtils.ConvertFromInternalUnits(result, DisplayUnitType.DUT_CUBIC_METERS);
            return result;
        }
    }
}