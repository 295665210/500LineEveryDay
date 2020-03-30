using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CodeInTangsengjiewa4.CodeOfQian.WpfEventDemo;
using WpfApps;

namespace CodeInTangsengjiewa4.CodeInFunny.FamilyLibrary
{
    [Transaction(TransactionMode.Manual)]
    class Commands : IExternalCommand
    {
        public static PreviewModel mainWindow = null;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Application app = commandData.Application.Application;
            Document doc = commandData.Application.ActiveUIDocument.Document;
            try
            {
                mainWindow = new PreviewModel(app, uiapp);
                //设置窗体的从属窗体
                WindowInteropHelper mainUi = new WindowInteropHelper(mainWindow)
                {
                    Owner = Process.GetCurrentProcess().MainWindowHandle
                };
                mainWindow.ShowDialog();
            }
            catch (Exception e)
            {
                TaskDialog.Show("错误", e.Message);
            }

            return Result.Succeeded;
        }
    }
}