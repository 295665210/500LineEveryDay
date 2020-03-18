using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CodeInTangsengjiewa.BinLibrary.Extensions;
using System;
using System.Windows;

namespace CodeInTangsengjiewa4.CodeOfQian
{
    /// <summary>
    /// dim twp point
    /// 这段代码有点太难了..........
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class Cmd_DimTwoPoint : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;
            var view = doc.ActiveView;
            ViewType viewType = view.ViewType;

            if (viewType == ViewType.FloorPlan || viewType == ViewType.Elevation)
            {
                Reference eRef = sel.PickObject(ObjectType.Element, "please select a curve based element like wall");
                Element element = eRef.GetElement(doc);
                if (eRef != null && element != null)
                {
                    XYZ viewNormal = view.ViewDirection;
                    LocationCurve locationCurve = element.Location as LocationCurve;
                    if (null == locationCurve || null == locationCurve.Curve)
                    {
                        MessageBox.Show("tips", "Selected element is not curve based");
                    }
                    XYZ dirCurve = locationCurve.Curve.GetEndPoint(0).Subtract(locationCurve.Curve.GetEndPoint(1))
                        .Normalize();
                    double d = dirCurve.DotProduct(viewNormal);
                    if (Math.Abs(d) < 1e-8)
                    {
                        var directionVec = dirCurve.CrossProduct(viewNormal);
                        XYZ p1 = locationCurve.Curve.GetEndPoint(0);
                        XYZ p2 = locationCurve.Curve.GetEndPoint(1);
                        XYZ dirLine = XYZ.Zero.Add(p1);
                        XYZ newVec = XYZ.Zero.Add(directionVec);
                        newVec = newVec.Normalize().Multiply(3);
                        dirLine = dirLine.Subtract(p2);
                        p1 = p1.Add(newVec);
                        p2 = p2.Add(newVec);
                        Line newLine = Line.CreateBound(p1, p2);
                        ReferenceArray arrRefs = new ReferenceArray();
                        Options options = new Options
                        {
                            ComputeReferences = true,
                            DetailLevel = ViewDetailLevel.Fine
                        };

                        GeometryElement gElement = element.get_Geometry(options);
                        foreach (var geometryObject in gElement)
                        {
                            Solid solid = geometryObject as Solid;
                            if (solid == null)
                            {
                                continue;
                            }
                            FaceArrayIterator forwardIterator = solid.Faces.ForwardIterator();
                            while (forwardIterator.MoveNext())
                            {
                                PlanarFace p = forwardIterator.Current as PlanarFace;
                                if (p == null)
                                {
                                    continue;
                                }
                                p2 = p.FaceNormal.CrossProduct(dirLine);
                                if (p2.IsZeroLength())
                                {
                                    arrRefs.Append(p.Reference);
                                }
                                if (2 == arrRefs.Size)
                                {
                                    break;
                                }
                            }
                            if (arrRefs.Size != 2)
                            {
                                MessageBox.Show("could not find enough reference for dimension");
                            }
                            Transaction ts = new Transaction(doc, "create dimension");
                            ts.Start();
                            doc.Create.NewDimension(doc.ActiveView, newLine, arrRefs);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("only support plan view or Elevation view.");
                }
            }
            return Result.Succeeded;
        }
    }
}