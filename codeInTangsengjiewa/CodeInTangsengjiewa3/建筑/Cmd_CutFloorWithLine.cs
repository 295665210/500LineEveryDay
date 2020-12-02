using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI.Selection;
using CodeInTangsengjiewa3.BinLibrary.Extensions;
using CodeInTangsengjiewa3.BinLibrary.Helpers;
using UIFrameworkServices;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace CodeInTangsengjiewa3.建筑
{
    /// <summary>
    /// 划线切板
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    class Cmd_CutFloorWithLine : IExternalCommand
    {
        private Floor floor = null;
        ICollection<ElementId> ids_add = new List<ElementId>();
        private Application App = null;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var app = uiapp.Application;
            App = app;
            var uidoc = uiapp.ActiveUIDocument;

            var doc = uidoc.Document;
            var sel = uidoc.Selection;

            var acview = doc.ActiveView;
            app.DocumentChanged += OnDocumentChanged;
            uiapp.Idling += OnIdling;

            floor = sel.PickObject(ObjectType.Element, doc.GetSelectionFilter(m => m is Floor))
                .GetElement(doc) as Floor;

#if Revit2019
            CommandHandlerService.invokeCommandHandler("ID_OBJECTS_PROJECT_CURVE");
#endif
#if Revit2016
            //调用 postablecommand
           var commandid = RevitCommandId.LookupPostableCommandId(PostableCommand.ModelLine);
            uiapp.PostCommand(commandid);

#endif

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

#if Revit2019
                leftspacePlane = Plane.CreateByNormalAndOrigin(leftNorm, startpo);
                rightspacePlane = Plane.CreateByNormalAndOrigin(rightNorm, startpo);

#endif

                var geoele = floor.get_Geometry(new Options()
                {
                    ComputeReferences = true, DetailLevel = ViewDetailLevel.Fine
                });

                var solid = geoele.GetSolids().FirstOrDefault();

                var newsolid1 = BooleanOperationsUtils.CutWithHalfSpace(solid, leftspacePlane);
                var newsolid2 = BooleanOperationsUtils.CutWithHalfSpace(solid, rightspacePlane);

                var upface1 = newsolid1.GetFacesOfGeometryObject()
                    .Where(m => m.ComputeNormal(new UV()).IsSameDirection(XYZ.BasisZ)).FirstOrDefault();
                var upfaces2 = newsolid2.GetFacesOfGeometryObject()
                    .Where(m => m.ComputeNormal(new UV()).IsSameDirection(XYZ.BasisZ)).FirstOrDefault();

                var curveloop1 = upface1.GetEdgesAsCurveLoops().FirstOrDefault();
                var curvearray1 = curveloop1.ToCurveArray();

                var curveloop2 = upfaces2.GetEdgesAsCurveLoops().FirstOrDefault();
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
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
                uiapp.Idling -= OnIdling;
            }
        }

        private void OnDocumentChanged(object sender, DocumentChangedEventArgs e)
        {
            var App = sender as Application;
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

#if Revit2019
                leftspacePlane = Plane.CreateByNormalAndOrigin(leftNorm, startpo);
                rightspacePlane = Plane.CreateByNormalAndOrigin(rightNorm, startpo);
#endif

                var slapshapeEditor = floor.SlabShapeEditor;

                var geoele = floor.get_Geometry(new Options()
                {
                    ComputeReferences = true, DetailLevel = ViewDetailLevel.Fine
                });

                var solid = geoele.GetSolids().FirstOrDefault();

                var newsolid1 = BooleanOperationsUtils.CutWithHalfSpace(solid, leftspacePlane);
                var newsolid2 = BooleanOperationsUtils.CutWithHalfSpace(solid, rightspacePlane);

                var upface1 = newsolid1.GetFacesOfGeometryObject()
                    .Where(m => m.ComputeNormal(new UV()).IsSameDirection(XYZ.BasisZ)).FirstOrDefault();
                var upfaces = newsolid2.GetFacesOfGeometryObject().Where(m => m.ComputeNormal(new UV()) == XYZ.BasisZ)
                    .FirstOrDefault();

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
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
                App.DocumentChanged -= OnDocumentChanged;
            }
        }
    }
}