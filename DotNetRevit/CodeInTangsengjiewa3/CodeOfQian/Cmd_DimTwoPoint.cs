using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CodeInTangsengjiewa3.BinLibrary.Extensions;

namespace CodeInTangsengjiewa3.CodeOfQian
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
            var uiapp = commandData.Application;
            var app = uiapp.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;
            var view = doc.ActiveView;
            ViewType vt = view.ViewType;

            if (vt == ViewType.FloorPlan || vt == ViewType.Elevation)
            {
                Reference eRef = sel.PickObject(ObjectType.Element, "Please select a curve based element like wall");
                Element element = eRef.GetElement(doc);
                if (eRef != null && element != null)
                {
                    XYZ dirVec = new XYZ();
                    XYZ viewNormal = view.ViewDirection;

                    LocationCurve locationCurve = element.Location as LocationCurve;
                    if (null == locationCurve || null == locationCurve.Curve)
                    {
                        MessageBox.Show("tips", "Selected element isnot curve based");
                    }
                    XYZ dirCurve = locationCurve.Curve.GetEndPoint(0).Subtract(locationCurve.Curve.GetEndPoint(1))
                        .Normalize();
                    double d = dirCurve.DotProduct(viewNormal);
                    if (Math.Abs(d) < 1e-8)
                    {
                        dirVec = dirCurve.CrossProduct(viewNormal);
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

                        GeometryElement gelement = element.get_Geometry(options);
                        foreach (var geometryObject in gelement)
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
                else
                {
                    MessageBox.Show("only support Plan view or Elevation view.");
                }
            }
            return Result.Succeeded;
        }
    }
}