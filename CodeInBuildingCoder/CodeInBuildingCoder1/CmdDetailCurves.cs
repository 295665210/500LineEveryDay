using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI.Selection;

namespace CodeInBuildingCoder1
{
    [Transaction(TransactionMode.Manual)]
    class CmdDetailCurves : IExternalCommand
    {
        /// <summary>
        /// Return a point projected onto a plane defined by its normal.
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        XYZ ProjectPointOntoPlane(XYZ point, XYZ planeNormal)
        {
            double a = planeNormal.X;
            double b = planeNormal.Y;
            double c = planeNormal.Z;

            double dx = (b * b + c * c) * point.X - (a * b) * point.Y - (a * c) * point.Z;
            double dy = -(b * a) * point.X + (a * a + c * c) * point.Y - (b * c) * point.Z;
            double dz = -(c * a) * point.X - (c * b) * point.Y + (a * a + b * b) * point.Z;

            return new XYZ(dx, dy, dz);
        }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            View view = doc.ActiveView;

            Autodesk.Revit.Creation.Document creDoc = doc.Create;

            #region Check for pre-selected wall element
            Selection sel = uidoc.Selection;
            ICollection<ElementId> ids = sel.GetElementIds();

            if (1 == ids.Count)
            {
                Element e = doc.GetElement(ids.First());
                if (e is Wall)
                {
                    LocationCurve lc = e.Location as LocationCurve;
                    Curve curve = lc.Curve;

                    using (Transaction tx = new Transaction(doc))
                    {
                        tx.Start("Create Detail Line in Wall Center.");
                        creDoc.NewDetailCurve(view, curve);
                        tx.Commit();
                    }
                    return Result.Succeeded;
                }
            }
            #endregion

            //Create a geometry line
            XYZ startPoint = new XYZ(0, 0, 0);
            XYZ endPoint = new XYZ(10, 10, 0);

            //
            Line geomLine = Line.CreateBound(startPoint, endPoint); //2014
            XYZ end0 = new XYZ(0, 0, 0);
            XYZ end1 = new XYZ(10, 0, 0);
            XYZ pointOnCurve = new XYZ(5, 5, 0);

            Arc geomArc = Arc.Create(end0, end1, pointOnCurve);

#if NEED_PLANE
//Create a geometry plane
            XYZ origin = new XYZ(0,0,0);
            XYZ normal = new XYZ(1,1,0);
            Plane geomPlane = creApp.NewPlane(normal,origin);
            // Create a sketch plane in current document
            SketchPlane sketch = creDoc.NewSketchPlane(geomPlane);

#endif
            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Create Detail Line and Arc");
                DetailLine line = creDoc.NewDetailCurve(view, geomLine) as DetailLine;
                DetailArc arc = creDoc.NewDetailCurve(view, geomArc) as DetailArc;

                GraphicsStyle gs = arc.LineStyle as GraphicsStyle;
                gs.GraphicsStyleCategory.LineColor = new Color(250, 10, 10);

                tx.Commit();
            }
            return Result.Succeeded;
        }
    }
}