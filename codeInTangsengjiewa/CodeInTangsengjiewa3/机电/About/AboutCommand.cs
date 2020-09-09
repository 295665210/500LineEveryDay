using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Interop;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CodeInTangsengjiewa3.机电.About
{
    [Transaction(TransactionMode.Manual)]
    class AboutCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            唐僧解瓦.机电.About.About about = new 唐僧解瓦.机电.About.About();
            var acwinHwd = Process.GetCurrentProcess().MainWindowHandle;
            var acwin = NativeWindow.FromHandle(acwinHwd);

            WindowInteropHelper winhHelper = new WindowInteropHelper(about);
            winhHelper.Owner = acwinHwd;
            about.Show();

            return Result.Succeeded;
        }
    }
}