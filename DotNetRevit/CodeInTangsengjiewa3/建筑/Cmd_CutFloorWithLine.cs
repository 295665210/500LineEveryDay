using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Application = Autodesk.Revit.ApplicationServices.Application;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI.Selection;
using CodeInTangsengjiewa3.BinLibrary.Extensions;
using CodeInTangsengjiewa3.BinLibrary.Helpers;
using UIFrameworkServices;

namespace CodeInTangsengjiewa3.建筑
{
    [Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Manual)]
    class Cmd_CutFloorWithLine : IExternalCommand
    {
        //为什么一些属性要定义在这里??
        private Floor floor = null;
        ICollection<ElementId> ids_add = new List<ElementId>();
        private Application App = null;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var app = uiapp.Application;
            App = app;
            var uidoc = commandData.Application.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;
            var acview = uidoc.ActiveView;

            app.DocumentChanged += OnDocumentChanged;
            uiapp.Idling += OnIdling;

            floor =
                sel.PickObject(ObjectType.Element, doc.GetSelectionFilter(m => m is Floor)).GetElement(doc) as Floor;
            CommandHandlerService.invokeCommandHandler("ID_OBJECTS_PROJECT_CURVE");
            return Result.Succeeded;
        }

        private void OnIdling(object sender, IdlingEventArgs e)
        {
            var docsInApp = App.Documents.Cast<Document>();
            var doc = docsInApp.FirstOrDefault();
            var uidoc = new UIDocument(doc);
            var uiapp = uidoc.Application;
            try
            {
                var ActiveDoc = docsInApp.FirstOrDefault();
                if (ids_add.Count != 1)
                {
                    return;
                }
                var modelline = ids_add.First().GetElement(ActiveDoc) as ModelLine;
                var line = modelline.GeometryCurve as Line;
                var linedir = line.Direction;
                var startpo = line.StartPoint();
                var endpo = line.EndPoint();

                var updir = XYZ.BasisZ;

                var leftNorm = updir.CrossProduct(linedir).Normalize();
                var rightNorm = updir.CrossProduct(-linedir).Normalize();

                var leftspacePlane = default(Plane);
                var rightspacePlane = default(Plane);

                leftspacePlane = Plane.CreateByNormalAndOrigin(leftNorm, startpo);
                rightspacePlane = Plane.CreateByNormalAndOrigin(rightNorm, startpo);

                var geoele = floor.get_Geometry(new Options()
                {
                    ComputeReferences = true, DetailLevel = ViewDetailLevel.Fine
                });

                var solid = geoele.GetSolids().FirstOrDefault();

                var newsolid1 = BooleanOperationsUtils.CutWithHalfSpace(solid, leftspacePlane);
                var newsolid2 = BooleanOperationsUtils.CutWithHalfSpace(solid, rightspacePlane);

                var upface1 = newsolid1.GetFacesOfGeometryObject()
                    .Where(m => m.ComputeNormal(new UV()).IsSameDirection(XYZ.BasisZ)).FirstOrDefault();
                var upface2 = newsolid2.GetFacesOfGeometryObject()
                    .Where(m => m.ComputeNormal(new UV()).IsSameDirection(XYZ.BasisZ)).FirstOrDefault();

                var curveloop1 = upface1.GetEdgesAsCurveLoops().FirstOrDefault();
                var curvearray1 = curveloop1.ToCurveArray();

                var curveloop2 = upface2.GetEdgesAsCurveLoops().FirstOrDefault();
                var curvearray2 = curveloop2.ToCurveArray();

                ActiveDoc.Invoke(m =>
                {
                    var newfloor1 = ActiveDoc.Create.NewFloor(curvearray1, floor.FloorType,
                                                              floor.LevelId.GetElement(ActiveDoc) as Level, false);
                    var newfloor2 = ActiveDoc.Create.NewFloor(curvearray2, floor.FloorType,
                                                              floor.LevelId.GetElement(ActiveDoc) as Level, false);
                    doc.Delete(floor.Id);
                }, "new floor");

                if (this != null)
                {
                    uiapp.Idling -= OnIdling;
                }
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.ToString());
                uiapp.Idling -= OnIdling;
            }
        }

        private void OnDocumentChanged(object sender, DocumentChangedEventArgs e)
        {
            var app = sender as Application;
            try
            {
                var docsInApp = App.Documents.Cast<Document>();
                var ActiveDoc = e.GetDocument();
                ids_add = e.GetAddedElementIds();
                if (ids_add.Count != 1)
                {
                    return;
                }
                var modelline = ids_add.First().GetElement(ActiveDoc) as ModelLine;
                var line = modelline.GeometryCurve as Line;

                var linedir = line.Direction;
                var startpo = line.StartPoint();
                var endpo = line.EndPoint();

                var updir = XYZ.BasisZ;

                var leftNorm = updir.CrossProduct(linedir).Normalize();
                var rightNorm = updir.CrossProduct(-linedir).Normalize();

                var leftspacePlane = default(Plane);
                var rightspacePlane = default(Plane);

                leftspacePlane = Plane.CreateByNormalAndOrigin(leftNorm, startpo);
                rightspacePlane = Plane.CreateByNormalAndOrigin(rightNorm, startpo);

                var slapshaperEditor = floor.SlabShapeEditor;
                //剪切楼板
                var geoele = floor.get_Geometry(new Options()
                {
                    ComputeReferences = true, DetailLevel = ViewDetailLevel.Fine
                });

                var solid = geoele.GetSolids().FirstOrDefault();

                var newsolid1 = BooleanOperationsUtils.CutWithHalfSpace(solid, leftspacePlane);
                var newsolid2 = BooleanOperationsUtils.CutWithHalfSpace(solid, rightspacePlane);

                var upface1 = newsolid1
                    .GetFacesOfGeometryObject()
                    .FirstOrDefault(m => m.ComputeNormal(new UV()).IsSameDirection(XYZ.BasisZ));
                var upface2 = newsolid2.GetFacesOfGeometryObject()
                    .FirstOrDefault(m => m.ComputeNormal(new UV()) == XYZ.BasisZ);

                var curveloop1 = upface1.GetEdgesAsCurveLoops().FirstOrDefault();
                var curvearray = curveloop1.ToCurveArray();
                ActiveDoc.Invoke(m =>
                {
                    var newfloor1 = ActiveDoc.Create.NewFloor(curvearray, floor.FloorType,
                                                              floor.LevelId.GetElement(ActiveDoc) as Level, false);
                }, "new floor");

                if (this != null)
                {
                    App.DocumentChanged -= OnDocumentChanged;
                }
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.ToString());
                App.DocumentChanged -= OnDocumentChanged;
            }
        }
    }
}