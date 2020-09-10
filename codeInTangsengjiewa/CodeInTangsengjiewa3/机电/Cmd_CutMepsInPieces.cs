using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CodeInTangsengjiewa3.BinLibrary.Extensions;

namespace CodeInTangsengjiewa3.机电
{
    /// <summary>
    /// 管线分段
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_CutMepsInPieces : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Reference r = uidoc.Selection.PickObject(ObjectType.PointOnElement, "请选择一个分段点");
            Element elem = doc.GetElement(r);
            MEPCurve mec = elem as MEPCurve;
            Line line = (mec.Location as LocationCurve).Curve as Line;
            XYZ splitpoint = line.Project(r.GlobalPoint).XYZPoint;
            Level level = doc.ActiveView.GenLevel;

            using (Transaction ts = new Transaction(doc))
            {
                try
                {
                    ts.Start("开始分段");
                    var collector = new FilteredElementCollector(doc, doc.ActiveView.Id);
                    var mepcollector = collector.OfClass(typeof(MEPCurve)).WhereElementIsNotElementType()
                        .Where(m => (m as MEPCurve).LocationLine().Length > 2100d.MetricToFeet()).Cast<MEPCurve>()
                        .ToList();
                    var perlength = 6000d.MetricToFeet();
                    var ductperlength = 4000d.MetricToFeet();
                    var cabletrayperlength = 2000d.MetricToFeet();
                    var pipeperlength = 6000d.MetricToFeet();

                    foreach (MEPCurve mep in mepcollector)
                    {
                        if (mep is CableTray)
                        {
                            perlength = cabletrayperlength;
                        }
                        else if (mep is Duct)
                        {
                            perlength = ductperlength;
                        }
                        else if (mep is Pipe)
                        {
                            perlength = pipeperlength;
                        }
                        var temline = mep.LocationLine();
                        var length = temline.Length;
                        if (length <= perlength)
                        {
                            continue;
                        }
                        var points = new List<XYZ>();
                        for (int i = 0; i < length/perlength; i++)
                        {
                            var tempoint = temline.StartPoint() + perlength * i * temline.Direction;
                            points.Add(tempoint);
                        }
                        var temmeplist =new List<ElementId>();
                        Breakm
                    }


                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }
}