﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using CodeInTangsengjiewa3.BinLibrary.Helpers;


namespace CodeInTangsengjiewa3.BinLibrary.Extensions
{
    public static class CreationExtension
    {
        public static void NewLine_withoutTransaction(this Document doc, Line line)
        {
            var dir = line.Direction;
            var origin = line.Origin;
            var norm = default(XYZ);

            norm = dir.getRandomNorm();
            var plane = default(Plane);
            plane = Plane.CreateByNormalAndOrigin(norm, origin);

            var sketchPlane = SketchPlane.Create(doc, plane);

            doc.Create.NewModelCurve(line, sketchPlane);
        }

        public static void NewLine(this Document doc, Line line)
        {
            var dir = line.Direction;
            var origin = line.Origin;
            var norm = default(XYZ);
            norm = dir.getRandomNorm();

            var plane = default(Plane);
            plane = Plane.CreateByNormalAndOrigin(norm, origin);

            doc.Invoke(m =>
            {
                var sketchplane = SketchPlane.Create(doc, plane);
                doc.Create.NewModelCurve(line, sketchplane);
            }, "aa");
        }


        public static void NewBox(this Document doc, BoundingBoxXYZ box)
        {
            var trans = box.Transform;
            var min = box.Min;
            var max = box.Max;
            var x = max.X - min.X;
            var y = max.Y - min.Y;
            var z = max.Z - min.Z;

            //1、
            var endx = min + x * trans.BasisX;
            var linex = Line.CreateBound(min, endx);

            var linex_1 = Line.CreateBound(endx, endx + y * trans.BasisY);
            var linex_2 = Line.CreateBound(endx, endx + z * trans.BasisZ);

            var endy = min + y * trans.BasisY;
            var liney = Line.CreateBound(min, endy);

            var liney_1 = Line.CreateBound(endy, endy + x * trans.BasisX);
            var liney_2 = Line.CreateBound(endy, endy + z * trans.BasisZ);

            var endz = min + z * trans.BasisZ;
            var linez = Line.CreateBound(min, endz);

            var linez_1 = Line.CreateBound(endz, endz + x * trans.BasisX);
            var linez_2 = Line.CreateBound(endz, endz + y * trans.BasisY);

            var _linex = Line.CreateBound(max, max - x * trans.BasisX);
            var _liney = Line.CreateBound(max, max - y * trans.BasisY);
            var _linez = Line.CreateBound(max, max - z * trans.BasisZ);
            doc.Invoke(m =>
            {
                doc.NewLine_withoutTransaction(linex);
                doc.NewLine_withoutTransaction(linex_1);
                doc.NewLine_withoutTransaction(linex_2);
                doc.NewLine_withoutTransaction(liney);
                doc.NewLine_withoutTransaction(liney_1);
                doc.NewLine_withoutTransaction(liney_2);
                doc.NewLine_withoutTransaction(linez);
                doc.NewLine_withoutTransaction(linez_1);
                doc.NewLine_withoutTransaction(linez_2);
                doc.NewLine_withoutTransaction(_linex);
                doc.NewLine_withoutTransaction(_liney);
                doc.NewLine_withoutTransaction(_linez);
            }, "创建包围框");
        }

        public static void NewCoordinate(this Document doc, XYZ po, Transform trs, double dis = 2)
        {
            var linex = Line.CreateBound(po, po + dis * trs.BasisX);
            var liney = Line.CreateBound(po, po + dis * trs.BasisY);
            var linez = Line.CreateBound(po, po + dis * trs.BasisZ);

            doc.Invoke(m =>
            {
                doc.NewLine_withoutTransaction(linex);
                doc.NewLine_withoutTransaction(liney);
                doc.NewLine_withoutTransaction(linez);
            }, "创建坐标");
        }
    }
}