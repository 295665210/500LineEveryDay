using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using QiShiLog;
using QiShiLog.Log;

namespace LogInRevit
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class LogTest : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // UIApplication uiapp = commandData.Application;
            // UIDocument uidoc = uiapp.ActiveUIDocument;
            // Document doc = uidoc.Document;
            // Selection sel = uidoc.Selection;
            // View acview = uidoc.ActiveView;

            Logger.Instance.EnableInfoFile = true;
            Logger.Instance.Info("日志4");
            Process.Start(Path.Combine(QiShiCore.WorkSpace.Dir, "log"));
         
            return Result.Succeeded;
        }
    }
}