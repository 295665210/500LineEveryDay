﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CodeInTangsengjiewa4.BinLibrary.Extensions;

namespace CodeInTangsengjiewa4.Test
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_InRegionTest : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;

            var points = new List<XYZ>();

            while (true)
            {
                try
                {
                    points.Add(sel.PickPoint());
                }
                catch (Exception e)
                {
                    message = e.ToString();
                    break;
                }
            }
            for (int i = 0; i < points.Count; i++)
            {
                if (i < points.Count - 1)
                {
                    var line = Line.CreateBound(points.ElementAt(i), points.ElementAt(i + 1));
                }
                else if (i == points.Count - 1)
                {
                    var line = Line.CreateBound(points.ElementAt(i), points.ElementAt(0));
                }
            }
            var point = sel.PickPoint();
            var temLine = Line.CreateBound(point, point + XYZ.BasisZ * 10);
            doc.NewLine(temLine);
            var result = IsPointInRegion(point, points, XYZ.BasisZ);
            MessageBox.Show(result.ToString());
            return Result.Succeeded;
        }

        public bool IsPointInRegion(XYZ po, List<XYZ> points)
        {
            bool result = false;
            var angles = 0.0;
            for (int i = 0; i < points.Count; i++)
            {
                if (i < points.Count - 1)
                {
                    var curPo = points.ElementAt(i);
                    var nextPo = points.ElementAt(i + 1);
                    var angle = Angle(curPo, nextPo, po);
                    angles += angle;
                }
                else if (i == points.Count - 1)
                {
                    var curPo = points.ElementAt(i);
                    var nextPo = points.ElementAt(0);
                    var angle = Angle(curPo, nextPo, po);
                }
            }
            if (angles.IsEqual(2 * Math.PI))
            {
                result = true;
            }
            else
            {
                return false;
            }
            return result;
        }

        public bool IsPointInRegion(XYZ po, List<XYZ> points, XYZ planeNorm)
        {
            bool result = false;
            var angles = 0.0;
            for (int i = 0; i < points.Count; i++)
            {
                if (i < points.Count - 1)
                {
                    var curPo = points.ElementAt(i);
                    var nextPo = points.ElementAt(i + 1);
                    var angle = Angle(curPo, nextPo, po, planeNorm);
                    angles += angle;
                }
                else if (i == points.Count - 1)
                {
                    var curPo = points.ElementAt(i);
                    var nextPo = points.ElementAt(0);
                    var angle = Angle(curPo, nextPo, po, planeNorm);
                    angles += angle;
                }
            }
            result = angles.IsEqual(2 * Math.PI);
            return result;
        }

        public double Angle(XYZ point1, XYZ point2, XYZ point0)
        {
            var vec1 = (point1 - point0).Normalize();
            var vec2 = (point1 - point0).Normalize();
            if (vec1.IsSameDirection(vec2))
            {
                return 0;
            }
            else if (vec1.IsOppositeDirection(vec2))
            {
                return Math.PI;
            }
            var normal = default(XYZ);
            normal = vec1.CrossProduct(vec2).Normalize();
            var angle = vec1.AngleOnPlaneTo(vec2, normal);
            if (angle > Math.PI)
            {
                angle = angle - 2 * Math.PI;
            }
            return angle;
        }

        public double Angle(XYZ point1, XYZ point2, XYZ point0, XYZ planeNorm)
        {
            var vec1 = (point1 - point0).Normalize();
            var vec2 = (point1 - point0).Normalize();
            if (vec1.IsSameDirection(vec2))
            {
                return 0;
            }
            else if (vec1.IsOppositeDirection(vec2))
            {
                return Math.PI;
            }
            var normal = default(XYZ);
            normal = vec1.CrossProduct(vec2).Normalize();
            var angle = vec1.AngleOnPlaneTo(vec2, normal);
            if (angle > Math.PI)
            {
                angle = angle - 2 * Math.PI;
            }
            return angle * (normal.DotProduct(planeNorm));
        }
    }
}