using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace AutoTagRooms_WF
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                Transaction documentTransaction =
                    new Transaction(commandData.Application.ActiveUIDocument.Document, "document");
                documentTransaction.Start();
                //create a new instance of  class roomsData
                RoomsData data = new RoomsData(commandData);
                System.Windows.Forms.DialogResult result;

                //create a form to display the information of rooms
                using (AutoTagRoomsForm roomsTagForm = new AutoTagRoomsForm(data))
                {
                    result = roomsTagForm.ShowDialog();
                }

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    documentTransaction.Commit();
                    return Result.Succeeded;
                }
                else
                {
                    documentTransaction.RollBack();
                    return Result.Cancelled;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}