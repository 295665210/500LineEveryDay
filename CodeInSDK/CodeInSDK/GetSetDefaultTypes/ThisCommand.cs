using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CodeInSDK.GetSetDefaultTypes.CS;

namespace CodeInSDK.GetSetDefaultTypes
{
    [Regeneration(RegenerationOption.Manual)]
    class ThisCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (!DockablePane.PaneExists(DefaultFamilyTypes.PaneId) ||
                !DockablePane.PaneExists(DefaultElementTypes.PaneId))
            {
                return Result.Failed;
            }

            UIApplication uiApp = commandData.Application;
            if (uiApp == null)
            {
                return Result.Failed;
            }

            DockablePane pane = uiApp.GetDockablePane(DefaultFamilyTypes.PaneId);
            pane.Show();

            DockablePane elemTypePane = uiApp.GetDockablePane(DefaultElementTypes.PaneId);
            elemTypePane.Show();

            if (ThisApplication.DefaultFamilyTypesPane != null)
            {
                ThisApplication.DefaultFamilyTypesPane.SetDocument(commandData.Application.ActiveUIDocument.Document);
            }

            if (ThisApplication.DefaultElementTypesPane != null)
            {
                ThisApplication.DefaultElementTypesPane.SetDocument(commandData.Application.ActiveUIDocument.Document);
            }

            return Result.Succeeded;
        }
    }
}