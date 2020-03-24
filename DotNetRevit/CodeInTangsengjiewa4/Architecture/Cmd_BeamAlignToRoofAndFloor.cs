// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
// using Autodesk.Revit.Attributes;
// using Autodesk.Revit.DB;
// using Autodesk.Revit.UI;
//
// namespace CodeInTangsengjiewa4.Architecture
// {[Transaction(TransactionMode.Manual)]
// [Regeneration(RegenerationOption.Manual)]
// [Journaling(JournalingMode.UsingCommandData)]
//     class Cmd_BeamAlignToRoofAndFloor:IExternalCommand
//     {
//         public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
//         {
//             var uiapp = commandData.Application;
//             var app = uiapp.Application;
//             var uidoc = uiapp.ActiveUIDocument;
//             var doc = uidoc.Document;
//             var sel = uidoc.Selection;
//
//             var acview = doc.ActiveView;
//             var IsAlignTopFace = false; //根据设置确定
//             var IsAlignBottomFace = true; //根据设置确定
//             var selectedIds = sel.GetElementIds();
//
//             var selectionCollector = new FilteredElementCollector(doc, selectedIds);
//             //将选择集作为容器
//             var beamFilter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFraming);
//             var roofFilter =new ElementCategoryFilter(BuiltInCategory.OST_Roofs);
//             var floorFilter =new ElementCategoryFilter(BuiltInCategory.OST_Floors);
//             var structuralFoundationFilter =new ElementCategoryFilter(BuiltInCategory.OST_Ramps);
//
//             var beamCollector = new FilteredElementCollector(doc, selectedIds).WhereElementIsNotElementType().WherePasses(beamFilter);
//             var roofCollector = new FilteredElementCollector(doc, selectedIds).WhereElementIsNotElementType().WherePasses(roofFilter);
//             var floorCollector = new FilteredElementCollector(doc, selectedIds).WhereElementIsNotElementType().WherePasses(floorFilter);
//             var structuralFoundationCollector = new FilteredElementCollector(doc, selectedIds).WhereElementIsNotElementType().WherePasses(structuralFoundationFilter);
//
//             //1 梁随屋面: 将与屋面在同一楼层的梁进行处理,使之紧贴屋面;
//             //1-1 获取 顶面或者底面的边界线
//             var floorFaces = default(IList<Reference>);
//             foreach (Floor floor in floorFaces)
//             {
//                 if (IsAlignBottomFace)
//                 {
//                     floorFaces = HostObjectUtils.GetBottomFaces(floor);
//                 }
//                 else if (IsAlignTopFace)
//                 {
//                     floorFaces = HostObjectUtils.GetTopFaces(floor);
//                 }
//                 //排除空引用
//                 floorFaces = floorFaces.Where(m => floor.GetGeometryObjectFromReference(m) as Face != null).ToList();
//
//                 if (floorFaces.Count == 0 || floorFaces == null)
//                 {
//                     continue;
//                 }
//                 //用屋面边线切断所有投影相交的梁
//                 foreach (FamilyInstance beam in beamCollector)
//                 {
//                     ///未写完
//                 }
//
//
//             }
//
//         }
//     }
// }
