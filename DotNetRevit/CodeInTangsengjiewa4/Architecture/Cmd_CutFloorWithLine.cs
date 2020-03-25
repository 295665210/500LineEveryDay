using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI.Selection;
using CodeInTangsengjiewa4.BinLibrary.Helpers;
using CodeInTangsengjiewa4.BinLibrary.Extensions;
using UIFrameworkServices;
using Application = Autodesk.Revit.ApplicationServices.Application;


namespace CodeInTangsengjiewa4.Architecture
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_CutFloorWithLine : IExternalCommand
    {
        //??
        private Floor _floor = null;
        private ICollection<ElementId> _idsAdd = new List<ElementId>();
        private Application _app = null;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var app = uiapp.Application;
            _app = app;
            var uidoc = commandData.Application.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;

            app.DocumentChanged += OnDocumentChanged;
            uiapp.Idling += OnIdling;

            _floor =
                sel.PickObject(ObjectType.Element, doc.GetSelectionFilter(m => m is Floor))
                    .GetElement(doc) as Floor;
            CommandHandlerService.invokeCommandHandler("ID_OBJECTS_PROJECT_CURVE");

            return Result.Succeeded;
        }

        private void OnIdling(object sender, IdlingEventArgs e)
        {
            var docsInApp = _app.Documents.Cast<Document>().ToList();
            var doc = docsInApp.FirstOrDefault();
            var uidoc = new UIDocument(doc);
            var uiapp = uidoc.Application;
            try
            {
                var activeDoc = docsInApp.FirstOrDefault();
                if (_idsAdd.Count != 1)
                {
                    return;
                }
                var modelLine = _idsAdd.First().GetElement(activeDoc) as ModelLine;
                var line = modelLine.GeometryCurve as Line;
                var lineDirection = line.Direction;
                var startPoint = line.StartPoint();
                var endPoint = line.EndPoint();

                var upDirection = XYZ.BasisZ;

                var leftNorm = upDirection.CrossProduct(lineDirection).Normalize();
                var rightNorm = upDirection.CrossProduct(-lineDirection).Normalize();

                var leftSpacePlane = default(Plane);
                var rightSpacePlane = default(Plane);

                leftSpacePlane = Plane.CreateByNormalAndOrigin(leftNorm, startPoint);
                rightSpacePlane = Plane.CreateByNormalAndOrigin(rightNorm, startPoint);

                var geometryElement = _floor.get_Geometry(new Options()
                {
                    ComputeReferences = true, DetailLevel = ViewDetailLevel.Fine
                });

                var solid = geometryElement.GetSolids().FirstOrDefault();

                var newSolid1 = BooleanOperationsUtils.CutWithHalfSpace(solid, leftSpacePlane);
                var newSolid2 = BooleanOperationsUtils.CutWithHalfSpace(solid, rightSpacePlane);

                var upFace1 = newSolid1
                    .GetFacesOfGeometryObject()
                    .FirstOrDefault(m => m.ComputeNormal(new UV()).IsSameDirection(XYZ.BasisZ));
                var upFace2 = newSolid2
                    .GetFacesOfGeometryObject()
                    .FirstOrDefault(m => m.ComputeNormal(new UV()).IsSameDirection(XYZ.BasisZ));

                var curveLoop1 = upFace1.GetEdgesAsCurveLoops().FirstOrDefault();
                var curveArray1 = curveLoop1.ToCurveArray();

                var curveLoop2 = upFace2.GetEdgesAsCurveLoops().FirstOrDefault();
                var curveArray2 = curveLoop2.ToCurveArray();

                activeDoc.Invoke(m =>
                {
                    var newFloor1 = activeDoc.Create.NewFloor(curveArray1, _floor.FloorType,
                                                              _floor.LevelId.GetElement(activeDoc) as Level, false);
                    var newFloor2 = activeDoc.Create.NewFloor(curveArray2, _floor.FloorType,
                                                              _floor.LevelId.GetElement(activeDoc) as Level, false);
                    doc.Delete(_floor.Id);
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
                var activeDoc = e.GetDocument();
                _idsAdd = e.GetAddedElementIds();
                if (_idsAdd.Count != 1)
                {
                    return;
                }
                var modelLine = _idsAdd.First().GetElement(activeDoc) as ModelLine;
                var line = modelLine.GeometryCurve as Line;

                var lineDirection = line.Direction;
                var startPoint = line.StartPoint();
                var endPoint = line.EndPoint();

                var upDirection = XYZ.BasisZ;

                var leftNorm = upDirection.CrossProduct(lineDirection).Normalize();
                var rightNorm = upDirection.CrossProduct(-lineDirection).Normalize();

                var leftSpacePlane = default(Plane);
                var rightSpacePlane = default(Plane);

                leftSpacePlane = Plane.CreateByNormalAndOrigin(leftNorm, startPoint);
                rightSpacePlane = Plane.CreateByNormalAndOrigin(rightNorm, startPoint);

                var slapShapeEditor = _floor.SlabShapeEditor;
                //剪切楼板
                var geometryElement = _floor.get_Geometry(new Options()
                {
                    ComputeReferences = true, DetailLevel = ViewDetailLevel.Fine
                });

                var solid = geometryElement.GetSolids().FirstOrDefault();

                var newSolid1 = BooleanOperationsUtils.CutWithHalfSpace(solid, leftSpacePlane);
                var newSolid2 = BooleanOperationsUtils.CutWithHalfSpace(solid, rightSpacePlane);

                var upFace1 = newSolid1
                    .GetFacesOfGeometryObject()
                    .FirstOrDefault(m => m.ComputeNormal(new UV()).IsSameDirection(XYZ.BasisZ));
                var upFace2 = newSolid2.GetFacesOfGeometryObject()
                    .FirstOrDefault(m => m.ComputeNormal(new UV()) == XYZ.BasisZ);

                var curveLoop1 = upFace1.GetEdgesAsCurveLoops().FirstOrDefault();
                var curveArray = curveLoop1.ToCurveArray();
                activeDoc.Invoke(m =>
                {
                    var newFloor1 = activeDoc.Create.NewFloor(curveArray, _floor.FloorType,
                                                              _floor.LevelId.GetElement(activeDoc) as Level, false);
                }, "new floor");

                if (this != null) _app.DocumentChanged -= OnDocumentChanged;
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.ToString());
                _app.DocumentChanged -= OnDocumentChanged;
            }
        }
    }
}