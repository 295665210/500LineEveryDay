using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CurvedBeam.View;

namespace CurvedBeam.Command
{
    [Transaction(TransactionMode.Manual)]
    class ExternalCommands:IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            CurvedBeamMainWindow mainWindow = new CurvedBeamMainWindow(commandData);
          
            mainWindow.ShowDialog();
            return Result.Succeeded;
        }
    }
}
