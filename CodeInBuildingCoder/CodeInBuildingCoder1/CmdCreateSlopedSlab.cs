using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CodeInBuildingCoder1
{
    [Transaction(TransactionMode.Manual)]
    public class CmdCreateSlopedSlab : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Create sloped slab."); //sloped: 倾斜的。
                double width = 19.684;
                double length = 59.0551;
                double height = 9.8425196;

                XYZ[] pts = new XYZ[]
                {
                    new XYZ(0.0, 0.0, height),
                    new XYZ(width, 0.0, height),
                    new XYZ(0, length, height)
                };

                CurveArray profile = uiapp.Application.Create.NewCurveArray();

                Line line = null;

                int n = pts.GetLength(0);

                XYZ q = pts[n - 1];

                foreach (XYZ p in pts)
                {
                    line = Line.CreateBound(q, p);
                    profile.Append(line);
                    q = p;
                }

                Level level = new FilteredElementCollector(doc)
                                  .OfClass(typeof(Level))
                                  .FirstOrDefault(e => e.Name.Equals("CreateSlopedSlab")) as Level;
                if (null == level)
                {
                    level = Level.Create(doc, height);
                    level.Name = "Sloped Slab";
                }
                Floor floor = doc.Create.NewSlab(profile, level, line, 0.5, true);
            }
            return Result.Succeeded;
        }
    }
}