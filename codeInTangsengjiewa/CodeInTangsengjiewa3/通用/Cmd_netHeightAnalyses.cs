using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CodeInTangsengjiewa3.BinLibrary.Extensions;
using CodeInTangsengjiewa3.BinLibrary.Helpers;

namespace CodeInTangsengjiewa3.通用
{
    /// <summary>
    /// 净高分析
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_netHeightAnalyses : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;

            var collector = new FilteredElementCollector(doc);

            // var rooms = collector.OfClass(typeof(Room));
            // var phases = doc.Phases;
            // foreach (Phase phase in phases)
            // {
            //     MessageBox.Show(phase.Name.ToString());
            //     return Result.Succeeded;
            // }

            //创建房间
            var rooms = new List<SpatialElement>();
            doc.Invoke(m => { CreateRooms(doc, doc.ActiveView.GenLevel, doc.Phases.get_Item(1)); }, "当前视图楼层创建房间");

            //用创建的房间进行标高分析
            rooms = doc.TCollector<SpatialElement>().ToList();
            MessageBox.Show(rooms.Count.ToString());

            var names = rooms.Select(m => m.Name);

            var nameString = string.Join("\n", names);

            MessageBox.Show(nameString);

            var geometrys = new List<GeometryObject>();

            foreach (SpatialElement spatialElement in rooms)
            {
                var geometry = default(GeometryObject);
                if (SpatialElementGeometryCalculator.IsRoomOrSpace(spatialElement))
                {
                    MessageBox.Show(spatialElement.Name + "is room or space");
                }
                else
                {
                    MessageBox.Show(spatialElement.Name + "is not room or space.");
                }

                try
                {
                    geometry = new SpatialElementGeometryCalculator(doc).CalculateSpatialElementGeometry(spatialElement)
                        .GetGeometry();
                }
                catch (Exception e)
                {
                    var boundaries = spatialElement.GetBoundarySegments(new SpatialElementBoundaryOptions());
                    MessageBox.Show(boundaries.Count.ToString());

                    foreach (var boundarySegements in boundaries)
                    {
                        foreach (var boundarySegement in boundarySegements)
                        {
                            var curve = boundarySegement.GetCurve() as Line;
                            if (curve == null)
                            {
                                MessageBox.Show("curve is null");
                                doc.NewLine(curve);
                            }
                        }
                    }
                    MessageBox.Show("wrong Message skip this loop");
                    continue;
                }

                //
                var geometrysl = new List<GeometryObject>() {geometry};
                doc.Invoke(m =>
                {
                    var directShape = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_GenericModel));
                    directShape.AppendShape(geometrysl);
                }, "创建内建模型");
            }

            return Result.Succeeded;
        }

        /// <summary>
        /// 指定楼层，根据topology创建房间
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="level"></param>
        /// <param name="curPhase"></param>
        /// <returns></returns>
        public List<Room> CreateRooms(Document doc, Level level, Phase curPhase)
        {
            var rooms = new List<Room>();
            var pology = doc.get_PlanTopology(level);
            var circuits = pology.Circuits;
            var newPhase = doc.Phases
                .Cast<Phase>().First(m => m.Name.Contains("新构造") || m.Name.ToLower().Contains("new"));
            if (newPhase == null)
            {
                return rooms;
            }

            foreach (PlanCircuit circuit in circuits)
            {
                var sheduleroom = doc.Create.NewRoom(newPhase);
                var room = doc.Create.NewRoom(sheduleroom, circuit);
                rooms.Add(room);
            }
            return rooms;
        }
    }
}