using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;
using Autodesk.Revit.UI;

namespace CodeInBuildingCoder1
{
    [Transaction(TransactionMode.Manual)]
    class CmdExportIfc : IExternalCommand
    {
        static Result ExportToIfc(Document doc)
        {
            Result r = Result.Failed;
            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Export IFC");
                string desktop_path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                IFCExportOptions opt = null;

                doc.Export(desktop_path, doc.Title, opt);

                tx.RollBack(); //为什么rollback？

                r = Result.Succeeded;
            }
            return r;
        }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            ExportToIfc(doc);
            return Result.Succeeded;
        }
    }
}