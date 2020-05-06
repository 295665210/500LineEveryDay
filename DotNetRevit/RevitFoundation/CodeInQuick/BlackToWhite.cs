using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitFoundation.CodeInQuick
{

    
    [Transaction(TransactionMode.Manual)]
    internal class BlackToWhite : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Application app = uiapp.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document document = uiapp.ActiveUIDocument.Document;

            if (app.BackgroundColor.Red == 0)
            {
                app.BackgroundColor = new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue);
            }
            else
            {
                app.BackgroundColor = new Color(0, 0, 0);
            }
            return Result.Succeeded;
        }
    }
}