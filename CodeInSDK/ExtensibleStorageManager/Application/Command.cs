using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using ExtensibleStorageManager.User;
using View = Autodesk.Revit.DB.View;

namespace ExtensibleStorageManager
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class command1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UICommand dialog = new UICommand(commandData.Application.ActiveUIDocument.Document,
                commandData.Application.ActiveAddInId.GetGUID().ToString());
            dialog.ShowDialog();

            return Result.Succeeded;
        }
    }
}