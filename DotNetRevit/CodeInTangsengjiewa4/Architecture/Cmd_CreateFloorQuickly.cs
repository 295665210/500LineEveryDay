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

    public static class TemUtils
    {
        public static List<Face> GetUpFaces(this Solid solid)
        {
            var upFaces = new List<Face>();
            var faces = solid.Faces;
            foreach (Face face in faces)
            {
                var normal = face.ComputeNormal(new UV());
                if (normal.IsSameDirection(XYZ.BasisZ))
                {
                    upFaces.Add(face);
                }
            }
            return upFaces;
        }

        public static List<Face> GetUpFaces(this GeometryObject geoObj)
        {
            var solids = geoObj.GetSolids();
            var upFaces = new List<Face>();
            foreach (var solid in solids)
            {
                var temUpFaces = solid.GetUpFaces();
                if (temUpFaces.Count > 0)
                {
                    upFaces.AddRange(temUpFaces);
                }
            }
            return upFaces;
        }

        public static List<Solid> GetSolids(this GeometryObject geoObj)
        {
            var solids = new List<Solid>();
            if (geoObj is Solid solid)
            {
                solids.Add(solid);
            }
            else if (geoObj is GeometryInstance geoInstance)
            {
                var geometryElement = geoInstance.SymbolGeometry;
                var enu = geometryElement.GetEnumerator();
                while (enu.MoveNext())
                {
                    var temGeoobj = enu.Current as GeometryObject;
                    solids.AddRange(GetSolids(geoObj));
                }
            }
            else if (geoObj is GeometryElement geoElement)
            {
                var enu = geoElement.GetEnumerator();
                while (enu.MoveNext())
                {
                    var temGeoobj = enu.Current as GeometryObject;
                    solids.AddRange(GetSolids(temGeoobj));
                }
            }
            return solids;
        }

        public static List<Solid> GetSolids(this GeometryObject geoObj, Transform trs)
        {
            var solids = new List<Solid>();
            if (geoObj is Solid solid)
            {
                if (trs != null || trs != Transform.Identity)
                {
                    solid = SolidUtils.CreateTransformed(solid, trs);
                }
                solids.Add(solid);
            }
            else if (geoObj is GeometryInstance geoInstance)
            {
                var transform = geoInstance.Transform;
                var symbolGeometry = geoInstance.SymbolGeometry;
                var enu = symbolGeometry.GetEnumerator();
                while (enu.MoveNext())
                {
                    var temGeoobj = enu.Current as GeometryObject;
                    solids.AddRange(GetSolids(temGeoobj, transform));
                }
            }
            else if (geoObj is GeometryElement geoElement)
            {
                var enu = geoElement.GetEnumerator();
                while (enu.MoveNext())
                {
                    var temGeoObj = enu.Current as GeometryObject;
                    solids.AddRange(GetSolids(temGeoObj, trs));
                }
            }
            return solids;
        }

        public static List<GeometryObject> GetSolids(this Element element)
        {
            var result = new List<GeometryObject>();
            var geometryEle = element.get_Geometry(new Options()
            {
                DetailLevel = ViewDetailLevel.Fine
            });
            var enu = geometryEle.GetEnumerator();
            while (enu.MoveNext())
            {
                var curGeoobj = enu.Current;
                if (curGeoobj is Solid solid)
                {
                    result.Add(solid);
                }
                else if (curGeoobj is GeometryInstance)
                {
                    result.AddRange(GetSolids(curGeoobj));
                }
                else if (curGeoobj is GeometryElement)
                {
                    result.AddRange(GetSolids(curGeoobj));
                }
            }
            return result;
        }

        public static CurveArray ToCurveArray(this CurveLoop curveLoop)
        {
            var result = new CurveArray();
            foreach (Curve curve in curveLoop)
            {
                result.Append(curve);
            }
            return result;
        }

        public static Solid MergeSolids(Solid solid1, Solid solid2)
        {
            var result = default(Solid);
            try
            {
                result = BooleanOperationsUtils.ExecuteBooleanOperation(solid1, solid2, BooleanOperationsType.Union);
            }
            catch
            {
                result= null;
            }
            return result;
        }

        public static Solid MergeSolids(List<Solid> solids)
        {
            var result = default(Solid);
            foreach (var solid in solids)
            {
                if (result == null)
                {
                    result = solid;
                }
                else
                {
                    var temSolid = MergeSolids(result, solid);
                    if (temSolid == null)
                    {
                        continue;
                    }
                    result = temSolid;
                }
            }
            return result;
        }
    }
}