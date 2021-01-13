using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Document = Autodesk.Revit.DB.Document;

namespace CodeInBuildingCoder1
{
    [Transaction(TransactionMode.Manual)]
    class test : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData, ref string message,
            ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Document doc = uiapp.ActiveUIDocument.Document;

            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Create model curve copies of anlytical model curves");
                Creator creator = new Creator(doc);
                Curve curve =
                    Line.CreateBound(new XYZ(0, 0, 0), new XYZ(10, 0, 0));
                Curve curve2 =
                    Line.CreateBound(new XYZ(0, 0, 0), new XYZ(0, 10, 0));

                Curve curve3 =
                    Line.CreateBound(new XYZ(0, 0, 0),
                                     new XYZ(-57.6, -49.1, 0));

                doc.Create.NewDetailCurve(doc.ActiveView, curve);
                doc.Create.NewDetailCurve(doc.ActiveView, curve2);
                doc.Create.NewDetailCurve(doc.ActiveView, curve3);

                tx.Commit();
            }

            return Result.Succeeded;
        }
    }
}