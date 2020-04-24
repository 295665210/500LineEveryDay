using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CodeInTangsengjiewa.BinLibrary.Helpers;

namespace CodeInTangsengjiewa4.CodeInHuanGS
{
    [Transaction(TransactionMode.Manual)]
    public class Cmd_CreateLevel : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document document = commandData.Application.ActiveUIDocument.Document;
            document.Invoke(m => { CreateLevel(document); }, "create level");
          

            return Result.Succeeded;
        }

        Level CreateLevel(Document document)
        {
            double elevation = 20;
            Level level = Level.Create(document, elevation);
            if (null == level)
            {
                throw new Exception("create a new level failed.");
            }
            level.Name = "new level";
            return level;
        }
    }
}