using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Document = Autodesk.Revit.Creation.Document;

namespace CodeInSDK
{
    class Creator
    {
        Autodesk.Revit.DB.Document _doc;

        //these are Autodesk.Revit.Creation objects!
        private Application _creapp;
        private Document _credoc;

        public Creator(Autodesk.Revit.DB.Document doc)
        {
            _doc = doc;
            _credoc = doc.Create;
            _creapp = doc.Application.Create;
        }

        public void DrawPolygon(List<XYZ> loop)
        {
            XYZ p1 = XYZ.Zero;
            XYZ q = XYZ.Zero;
            bool first = true;
            foreach (XYZ p in loop)
            {
                if (first)
                {
                    p1 = p;
                    first = false;
                }
                else
                {
                    CreateModelLine(_doc, p, q);
                }
                q = p;
            }
            CreateModelLine(_doc, q, p1);
        }

        public static ModelLine CreateModelLine(Autodesk.Revit.DB.Document doc, XYZ p, XYZ q)
        {
            if (p.DistanceTo(q) < Util.MinLineLength)
            {
                return null;
            }

            XYZ v = q - p;

            double dxy = Math.Abs(v.X) + Math.Abs(v.Y);

            XYZ w = (dxy > Util.TolPointOnPlane) ? XYZ.BasisZ : XYZ.BasisY;

            XYZ norm = v.CrossProduct(w).Normalize();

            Plane plane = Plane.CreateByNormalAndOrigin(norm, p);

            SketchPlane sketchPlane = SketchPlane.Create(doc, plane);

            Line line = Line.CreateBound(p, q);

            ModelCurve curve = doc.IsFamilyDocument
                ? doc.FamilyCreate.NewModelCurve(line, sketchPlane)
                : doc.Create.NewModelCurve(line, sketchPlane);
            return curve as ModelLine;
        }
    }
}