﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CodeInTangsengjiewa3.BinLibrary.Extensions;
using CodeInTangsengjiewa3.BinLibrary.Helpers;

namespace CodeInTangsengjiewa3.CodeOfQian
{
    /// <summary>
    /// dim two wall
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class Cmd_DimWall : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var app = uiapp.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;

            Wall wall = sel.PickObject(ObjectType.Element).GetElement(doc) as Wall;

            if (null != wall)
            {
                ReferenceArray refArray = new ReferenceArray();
                Line wallLine = (wall.Location as LocationCurve).Curve as Line;
                XYZ wallDir = wallLine.Direction;

                Options opt = new Options();
                opt.ComputeReferences = true;
                opt.DetailLevel = ViewDetailLevel.Fine;
                GeometryElement geoEle = wall.get_Geometry(opt);
                foreach (GeometryObject obj in geoEle)
                {
                    if (obj is Solid)
                    {
                        Solid solid = obj as Solid;
                        foreach (Face face in solid.Faces)
                        {
                            if (face is PlanarFace)
                            {
                                XYZ faceDir = face.ComputeNormal(new UV());
                                if (faceDir.IsAlmostEqualTo(wallDir) || faceDir.IsAlmostEqualTo(-wallDir))
                                {
                                    refArray.Append(face.Reference);
                                }
                            }
                        }
                    }
                }
                doc.Invoke(m => { doc.Create.NewDimension(doc.ActiveView, wallLine, refArray); },
                           "dim the length of wall");
            }
            return Result.Succeeded;
        }
    }
}