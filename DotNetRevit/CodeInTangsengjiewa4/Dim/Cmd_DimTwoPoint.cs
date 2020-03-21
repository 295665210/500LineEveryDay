using System;
using System.Windows;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CodeInTangsengjiewa4.BinLibrary.Extensions;


namespace CodeInTangsengjiewa4.Dim
{
    /// <summary>
    /// dim two point
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class Cmd_DimTwoPoint : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            View view = doc.ActiveView;
            Selection sel = uidoc.Selection;
            ViewType vt = view.ViewType;
            
            if (vt == ViewType.FloorPlan || vt == ViewType.Elevation)
            {
                Reference eRef = default(Reference);
                try
                {
                    eRef = sel.PickObject(ObjectType.Element, "please select a curved based element like wall");

                }
                catch
                {
                }
                if (eRef == null)
                {
                    return Result.Cancelled;
                }
                Element element = eRef.GetElement(doc);
                if (eRef != null && element != null)
                {
                    XYZ viewNormal = view.ViewDirection;

                    LocationCurve locationCurve = element.Location as LocationCurve;
                    if (null == locationCurve || null == locationCurve.Curve)
                    {
                        MessageBox.Show("tips", "selected element is not curve based");
                    }
                    XYZ dirCurve = locationCurve.Curve.GetEndPoint(0).Subtract(locationCurve.Curve.GetEndPoint(1))
                        .Normalize();
                    double d = dirCurve.DotProduct(viewNormal);
                    if (Math.Abs(d) < 1e-8)
                    {
                        var dirVec = dirCurve.CrossProduct(viewNormal);
                        XYZ p1 = locationCurve.Curve.GetEndPoint(0);
                        XYZ p2 = locationCurve.Curve.GetEndPoint(1);
                        XYZ dirLine = XYZ.Zero.Add(p1);
                        XYZ newVec = XYZ.Zero.Add(dirVec);
                        newVec = newVec.Normalize().Multiply(3);
                        dirLine = dirLine.Subtract(p2);
                        p1 = p1.Add(newVec);
                        p2 = p2.Add(newVec);

                        Line newLine = Line.CreateBound(p1, p2);
                        ReferenceArray arrRefs = new ReferenceArray();
                        Options options = new Options();
                        options.ComputeReferences = true;
                        options.DetailLevel = ViewDetailLevel.Fine;

                        GeometryElement gElement = element.get_Geometry(options);
                        foreach (var geometryObject in gElement)
                        {
                            Solid solid = geometryObject as Solid;
                            if (solid == null)
                            {
                                continue;
                            }
                            FaceArrayIterator fItor = solid.Faces.ForwardIterator();
                            while (fItor.MoveNext())
                            {
                                PlanarFace p = fItor.Current as PlanarFace;
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
                            ts.Commit();
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("only support Plan view or Elevation view .");
            }
            return Result.Succeeded;
        }
    }
}