using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using View = Autodesk.Revit.DB.View;

namespace ExportFamilys
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class ExternalCommands : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            View acview = uidoc.ActiveView;

            ExportFamilyMainWindow mainWindow = new ExportFamilyMainWindow(commandData);
            mainWindow.ShowDialog();
            return Result.Succeeded;
        }
    }
}