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

namespace ElementsBatchCreation
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Result res = Result.Succeeded;
            try
            {
                ElementsBatchCreation elementsBatchCreation = new ElementsBatchCreation(commandData);
                elementsBatchCreation.CreatElements();
            }
            catch (Exception)
            {
                message = "Batch creation failed";
                res = Result.Failed;
            }
            return res;
        }
    }
}