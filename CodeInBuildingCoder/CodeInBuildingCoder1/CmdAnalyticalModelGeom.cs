﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace CodeInBuildingCoder1
{
    [Transaction(TransactionMode.Manual)]
    public class CmdAnalyticalModelGeom : IExternalCommand
    {
        /// <summary>
        /// A list of all analytical curve types.
        /// </summary>
        static IEnumerable<AnalyticalCurveType> _curveTypes =
            Enum.GetValues(typeof(AnalyticalCurveType)).Cast<AnalyticalCurveType>();

        /// <summary>
        /// Offset at which to create a model curve copy
        /// of all analytical model curves.
        /// </summary>
        static XYZ _offset = new XYZ(100, 0, 0);

        /// <summary>
        /// Translation transformation to apply to create
        /// model curve copy of analytical model curves.
        /// </summary>
        static Transform _t = Transform.CreateTranslation(_offset);


        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            List<Element> walls = new List<Element>();

            if (!Util.GetSelectedElementsOrAll(walls, uidoc, typeof(Wall)))
            {
                Selection sel = uidoc.Selection;
                message = (0 < sel.GetElementIds().Count)
                              ? "Please select some wall elements."
                              : "No Wall elements found.";
                return Result.Failed;
            }

            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Create model curve copies of anlytical model curves");
                Creator creator = new Creator(doc);

                foreach (Wall wall in walls)
                {
                    AnalyticalModel am = wall.GetAnalyticalModel();

                    foreach (AnalyticalCurveType ct in _curveTypes)
                    {
                        IList<Curve> curves = am.GetCurves(ct);
                        int n = curves.Count;

                        Debug.Print("{0} {1} cureve {2}", n, ct, Util.PluralSuffix(n));

                        foreach (Curve curve in curves)
                        {
                            creator.CreateModelCurve(curve.CreateTransformed(_t));
                        }
                    }
                }
                tx.Commit();
            }

            return Result.Succeeded;
        }
    }
}