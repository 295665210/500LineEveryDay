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
    class CmdDocumentVersion : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData, ref string message,
            ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            string path = doc.PathName;
            BasicFileInfo info = BasicFileInfo.Extract(path);
            DocumentVersion v = info.GetDocumentVersion();
            int n = v.NumberOfSaves;
            Util.InfoMsg($"Document '{path}' has Guid {v.VersionGUID} and {n} save {Util.PluralSuffix(n)}");
            return Result.Succeeded;
        }
    }
}