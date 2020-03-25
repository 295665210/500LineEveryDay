using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;


namespace CodeInTangsengjiewa4.CodeOfQian.WpfRevitFilter
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class MainWindow : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            MainWindowRevitFilter mainWindow = new MainWindowRevitFilter();
            mainWindow.Show();
            return Result.Succeeded;
        }
    }
}