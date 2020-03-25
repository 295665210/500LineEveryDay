using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CodeInTangsengjiewa4.BinLibrary.Extensions;
using CodeInTangsengjiewa4.BinLibrary.Helpers;
using CodeInTangsengjiewa4.General.UIs;

namespace CodeInTangsengjiewa4.Architecture
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_CreateFloorQuickly : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;

            var floorTypes = doc.TCollector<FloorType>().ToList();
            var selectorUI = FloorSelector.Instance;
            selectorUI.FloorBox.ItemsSource = floorTypes;
            selectorUI.FloorBox.DisplayMemberPath = "Name";
            selectorUI.FloorBox.SelectedIndex = 0;
            selectorUI.ShowDialog();

            var targetFloorType = selectorUI.FloorBox.SelectedItem as FloorType;
            var beamRefs = default(IList<Reference>);
            try
            {
                beamRefs = sel.PickObjects(ObjectType.Element,
                                           doc.GetSelectionFilter(m => m.Category.Id.IntegerValue ==
                                                                       (int) BuiltInCategory.OST_StructuralFraming),
                                           "请选择生成板的梁");
            }
            catch (Exception e)
            {
                message = e.ToString();
                MessageBox.Show("用户取消了命令");
                return Result.Cancelled;
            }
            var beams = beamRefs.Select(m => m.GetElement(doc)).ToList();
            Transaction temTran = new Transaction(doc, "temTran");
            temTran.Start();
            foreach (Element beam in beams)
            {
                var joinedElements = JoinGeometryUtils.GetJoinedElements(doc, beam);
                if (joinedElements.Count > 0)
                {
                    foreach (var id in joinedElements)
                    {
                        var temEle = id.GetElement(doc);
                        var isJoined = JoinGeometryUtils.AreElementsJoined(doc, beam, temEle);
                    }
                }
            }
            temTran.RollBack();
            var solids = new List<GeometryObject>();
            foreach (var element in beams)
            {
                solids.AddRange(element.GetSolids());
            }
            var joinedSolid = TemUtils.MergeSolids(solids.Cast<Solid>().ToList());
            var upFaces = joinedSolid.GetUpFaces();
            var curveLoops = upFaces.First().GetEdgesAsCurveLoops();
            var orderedCurveLoops = curveLoops.OrderBy(m => m.GetExactLength()).ToList();
            orderedCurveLoops.RemoveAt(orderedCurveLoops.Count - 1);
            curveLoops = orderedCurveLoops;
            var curveArrays = curveLoops.Select(m => m.ToCurveArray());
            doc.Invoke(m =>
            {
                foreach (var curveArray in curveArrays)
                {
                    doc.Create.NewFloor(curveArray, false);
                }
            }, "一键成板");
            return Result.Succeeded;
        }
    }


}