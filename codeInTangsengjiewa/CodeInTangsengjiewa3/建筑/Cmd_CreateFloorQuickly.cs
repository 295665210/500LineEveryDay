using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Visual;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CodeInTangsengjiewa3.BinLibrary.Extensions;
using CodeInTangsengjiewa3.BinLibrary.Helpers;
using 唐僧解瓦.通用.UIs;

namespace CodeInTangsengjiewa3.建筑
{
    /// <summary>
    /// 一键成板（选择梁，根据梁围成的闭合区域生成板）
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_CreateFloorQuickly : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;

            var acview = doc.ActiveView;
            var floorTypes = doc.TCollector<FloorType>().ToList();

            var selectorUI = FloorTypeSelector.Instance;
            selectorUI.floortypeBox.ItemsSource = floorTypes;
            selectorUI.floortypeBox.DisplayMemberPath = "Name";
            selectorUI.floortypeBox.SelectedIndex = 0;
            selectorUI.ShowDialog();

            var targetFloorType = selectorUI.floortypeBox.SelectedItem as FloorType; //目标楼板类型

            var beamrefs = default(IList<Reference>);
            try
            {
                beamrefs = sel.PickObjects(ObjectType.Element,
                    doc.GetSelectionFilter(m =>
                        m.Category.Id.IntegerValue == (int) BuiltInCategory.OST_StructuralFraming), "选择生成板的梁");
            }
            catch (Exception e)
            {
                MessageBox.Show("用户取消了命令！");
                return Result.Cancelled;
            }
            var beams = beamrefs.Select(m => m.GetElement(doc));

            Transaction temtran = new Transaction(doc, "temtran");
            temtran.Start();
            foreach (Element beam in beams)
            {
                var joinedElements = JoinGeometryUtils.GetJoinedElements(doc, beam);
                if (joinedElements.Count > 0)
                {
                    foreach (var id in joinedElements)
                    {
                        var temele = id.GetElement(doc);
                        var isjoined = JoinGeometryUtils.AreElementsJoined(doc, beam, temele);
                        if (isjoined)
                        {
                            JoinGeometryUtils.UnjoinGeometry(doc, beam, temele);
                        }
                    }
                }
            }

            temtran.RollBack();

            var solids = new List<GeometryObject>();
            foreach (var element in beams)
            {
                solids.AddRange(element.Getsolids());
            }
            var joinedsolid = MergeSolids(solids.Cast<Solid>().ToList());

            var upfaces = joinedsolid.GetupFaces();
            var edgeArrays = upfaces.First().EdgeLoops.Cast<EdgeArray>().ToList();
            var curveloops = upfaces.First().GetEdgesAsCurveLoops();

            var orderedcurveloops = curveloops.OrderBy(m => m.GetExactLength()).ToList();
            orderedcurveloops.RemoveAt(orderedcurveloops.Count - 1);
            curveloops = orderedcurveloops;

            var curvearrays = curveloops.Select(m => m.ToCurveArray());

            doc.Invoke(m =>
            {
                foreach (var curveArray in curvearrays)
                {
                    doc.Create.NewFloor(curveArray, false);
                }
            }, "一键成板");

            return Result.Succeeded;
        }


        public Solid MergeSolids(Solid solid1, Solid solid2)
        {
            var result = default(Solid);
            try
            {
                result = BooleanOperationsUtils.ExecuteBooleanOperation(solid1, solid2, BooleanOperationsType.Union);
            }
            catch (Exception e)
            {
                result = null;
            }
            return result;
        }

        public Solid MergeSolids(List<Solid> solids)
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
                    var temsolid = MergeSolids(result, solid);
                    if (temsolid == null)
                        continue;
                    result = temsolid;
                }
            }
            return result;
        }
    }

    public static class TemUtils
    {
        public static List<Face> Getupfaces(this Solid solid)
        {
            var upfaces = new List<Face>();
            var faces = solid.Faces;
            foreach (Face face in faces)
            {
                var normal = face.ComputeNormal(new UV());
                if (normal.IsSameDirection(XYZ.BasisZ))
                {
                    upfaces.Add(face);
                }
            }
            return upfaces;
        }

        public static List<Face> GetupFaces(this GeometryObject geoele)
        {
            var solids = geoele.GetSolids();
            var upfaces = new List<Face>();

            foreach (var solid in solids)
            {
                var tempupfaces = solid.GetupFaces();
                if (tempupfaces.Count > 0)
                {
                    upfaces.AddRange(tempupfaces);
                }
            }
            return upfaces;
        }

        public static List<Solid> GetSolids(this GeometryObject geoobj)
        {
            var solids = new List<Solid>();
            if (geoobj is Solid solid)
            {
                solids.Add(solid);
            }
            else if (geoobj is GeometryInstance geoInstance)
            {
                var transform = geoInstance.Transform;
                var symbolgeometry = geoInstance.SymbolGeometry;
                var enu = symbolgeometry.GetEnumerator();
                while (enu.MoveNext())
                {
                    var temgeoobj = enu.Current as GeometryObject;
                    solids.AddRange(GetSolids(temgeoobj));
                }
            }

            else if (geoobj is GeometryElement geoElement)
            {
                var enu = geoElement.GetEnumerator();
                while (enu.MoveNext())
                {
                    var temgeoobj = enu.Current as GeometryObject;
                    solids.AddRange(GetSolids(temgeoobj));
                }
            }
            return solids;
        }

        public static List<Solid> GetSolids(this GeometryObject geoobj, Transform trs)
        {
            var solids = new List<Solid>();
            if (geoobj is Solid solid)
            {
                if (trs != null || trs != Transform.Identity)
                {
                    solid = SolidUtils.CreateTransformed(solid, trs);
                }
                solids.Add(solid);
            }

            else if (geoobj is GeometryInstance geoInstance)
            {
                var transform = geoInstance.Transform;
                var symbolgeometry = geoInstance.SymbolGeometry;
                var enu = symbolgeometry.GetEnumerator();
                while (enu.MoveNext())
                {
                    var temgeoobj = enu.Current as GeometryObject;
                    solids.AddRange(GetSolids(temgeoobj, transform));
                }
            }
            else if (geoobj is GeometryElement geoElement)
            {
                var enu = geoElement.GetEnumerator();
                while (enu.MoveNext())
                {
                    var temgeoobj = enu.Current as GeometryObject;
                    solids.AddRange(GetSolids(temgeoobj, trs));
                }
            }
            return solids;
        }

        /// <summary>
        /// 从元素获取solids 
        /// </summary>
        public static List<GeometryObject> Getsolids(this Element element)
        {
            var result = new List<GeometryObject>();
            var geometryEle = element.get_Geometry(new Options() {DetailLevel = ViewDetailLevel.Fine});
            var enu = geometryEle.GetEnumerator();
            while (enu.MoveNext())
            {
                var curGeoobj = enu.Current;
                result.AddRange(curGeoobj.GetSolids(Transform.Identity));
            }
            return result;
        }

        public static CurveArray ToCurveArray(this CurveLoop curveloop)
        {
            var result = new CurveArray();
            foreach (Curve curve in curveloop)
            {
                result.Append(curve);
            }
            return result;
        }
    }
}