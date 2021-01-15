using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CodeInBuildingCoder1
{
    [Transaction(TransactionMode.Manual)]
    class CmdCreateGableWall : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData, ref string message,
            ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Application app = uiapp.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            XYZ[] pts = new XYZ[]
            {
                XYZ.Zero,
                new XYZ(20, 0, 0),
                new XYZ(20, 0, 15),
                new XYZ(10, 0, 30),
                new XYZ(0, 0, 15)
            };
            //Get application creation object.
            Autodesk.Revit.Creation.Application appCreation = app.Create;

            List<Curve> profile = new List<Curve>(pts.Length);
            XYZ q = pts[pts.Length - 1];

            foreach (XYZ p in pts)
            {
                profile.Add(Line.CreateBound(q, p));
                q = p;
            }

            XYZ normal = XYZ.BasisY;

            WallType wallType =
                new FilteredElementCollector(doc).OfClass(typeof(WallType))
                    .FirstOrDefault() as WallType;
            Level level =
                new FilteredElementCollector(doc).OfClass(typeof(Level))
                    .First<Element>(e => e.Name.Equals("Level 1")) as Level;

            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Create Gable Wall");
                Wall wall = Wall.Create(doc, profile, wallType.Id, level.Id,
                                        true, normal);
                tx.Commit();
            }
            return Result.Succeeded;
        }
    }
}