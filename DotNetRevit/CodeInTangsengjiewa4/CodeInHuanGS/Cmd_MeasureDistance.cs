using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CodeInTangsengjiewa4.BinLibrary.Extensions;
using System;
using System.Collections.Generic;

namespace CodeInTangsengjiewa4.CodeInHuanGS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_MeasureDistance : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Application app = uiapp.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            //选择一个点
            Reference refPoint = sel.PickObject(ObjectType.PointOnElement);
            XYZ point1 = refPoint.GlobalPoint; // the position on which the reference is hit
            //射线方向及工作平面法向量
            XYZ rayDirection = XYZ.BasisZ;
            XYZ skVector = XYZ.BasisX; //暂定的值
            //当选择的主体为平面时,射线根据选择的点与此面的法线方向进行放射
            if (refPoint.ElementReferenceType == ElementReferenceType.REFERENCE_TYPE_SURFACE)
            {
                PlanarFace pFace = null;
                //主体是链接的图元时,获取平面的方法
                if (refPoint.LinkedElementId.IntegerValue != -1)
                {
                    RevitLinkInstance linkInstance = doc.GetElement(refPoint) as RevitLinkInstance;
                    Document linkDocument = linkInstance.GetLinkDocument();
                    Element linkElement = linkDocument.GetElement(refPoint.LinkedElementId);
                    Options opt = new Options();
                    opt.DetailLevel = ViewDetailLevel.Fine;
                    GeometryElement geoElem = linkElement.get_Geometry(opt);
                    pFace = GetTargetFace(geoElem, point1);
                }
                else
                {
                    //判断是否FamilyInstance类型的族,采用不同的获取方法
                    Element elem = doc.GetElement(refPoint);
                    if (elem is FamilyInstance)
                    {
                        Options opt = new Options();
                        opt.DetailLevel = ViewDetailLevel.Fine;
                        GeometryElement ge = elem.get_Geometry(opt);
                        pFace = GetTargetFace(ge, point1);
                    }
                    else
                    {
                        pFace = elem.GetGeometryObjectFromReference(refPoint) as PlanarFace;
                    }
                }
                //修正射线方向和工作平面法向量
                if (pFace != null)
                {
                    rayDirection = pFace.FaceNormal;
                    skVector = pFace.XVector;
                }
            }
            //获得视图
            View3D view3d = doc.ActiveView as View3D;

            //创建射线测量出第二点
            ExclusionFilter filter =
                new ExclusionFilter(new List<ElementId>() {refPoint.ElementId, refPoint.LinkedElementId});
            ReferenceIntersector referenceIntersector =
                new ReferenceIntersector(filter, FindReferenceTarget.All, view3d);
            ReferenceWithContext rwc = referenceIntersector.FindNearest(point1, rayDirection);
            if (rwc != null)
            {
                XYZ point2 = rwc.GetReference().GlobalPoint;
                //创建模型线
                Line line = Line.CreateBound(point1, point2);
                TaskDialog.Show("距离", line.Length.FeetToMm().ToString("0.00"));
                using (Transaction ts = new Transaction(doc, "尺寸"))
                {
                    ts.Start();
                    SketchPlane sk = SketchPlane.Create(doc, Plane.CreateByNormalAndOrigin(skVector, point1));
                    ModelCurve modelCurve = doc.Create.NewModelCurve(line, sk);
                    sel.SetElementIds(new List<ElementId>() {modelCurve.Id});
                    ts.Commit();
                }
            }
            else
            {
                TaskDialog.Show("测量得到的距离", "未检测到图元");
            }
            return Result.Succeeded;
        }

        public PlanarFace GetTargetFace(GeometryElement geometryElement, XYZ point)
        {
            PlanarFace face = null;
            //遍历几何元素里的geometryObject
            foreach (GeometryObject geometryObject in geometryElement)
            {
                //将geometryObject强转为Solid
                Solid solid = geometryObject as Solid;
                //遍历solid的面
                if (solid != null && solid.Faces.Size > 0)
                {
                    foreach (Face f in solid.Faces)
                    {
                        PlanarFace pFace = f as PlanarFace;
                        if (pFace != null)
                        {
                            try
                            {
                                //获取点到面的距离为0 的面
                                if (Math.Abs(pFace.Project(point).Distance) < 1e-6)
                                {
                                    face = pFace;
                                }
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                }
                if (face != null)
                {
                    break;
                }
            }
            if (face == null)
            {
                foreach (GeometryObject geometryObject in geometryElement)
                {
                    GeometryInstance geoIns = geometryObject as GeometryInstance;
                    if (geoIns != null)
                    {
                        face = GetTargetFace(geoIns.GetInstanceGeometry(), point);
                    }
                }
            }
            return face;
        }
    }
}