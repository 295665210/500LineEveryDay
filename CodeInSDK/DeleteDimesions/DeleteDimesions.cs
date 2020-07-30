using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using View = Autodesk.Revit.DB.View;

namespace DeleteDimesions
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class DeleteDimesions : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Selection selection = commandData.Application.ActiveUIDocument.Selection;
            Document document = commandData.Application.ActiveUIDocument.Document;

            ElementSet selections = new ElementSet();
            foreach (var elementId in selection.GetElementIds())
            {
                selections.Insert(document.GetElement(elementId));

            }

            ElementSet dimsToDelete = new ElementSet();

            //warning if nothing selected
            if (0 == selections.Size)
            {
                message = "please select dimensions";
                return Result.Failed;
            }
            //find all unpinned dimensions in the current selection
            foreach (Element e in selections)
            {
                Dimension dimensionTemp =e as Dimension;
                if (null != dimensionTemp && !dimensionTemp.Pinned)
                {
                    dimsToDelete.Insert(dimensionTemp);
                }

            }

            Transaction transaction =new Transaction(document,"External tool");
            transaction.Start();
            //delete all the unpinned dimensions
            foreach (Element e in dimsToDelete)
            {
                document.Delete(e.Id);
            }
            transaction.Commit();
            return Result.Succeeded;

        }
    }
}