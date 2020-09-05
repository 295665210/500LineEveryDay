using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Windows;
using System.Windows.Forms;

namespace CodeInTangsengjiewa3.BinLibrary.RevitHelper
{
    public static class RevitWindowHelper
    {
        public static IntPtr GetRevitHandle()
        {
            return Autodesk.Windows.ComponentManager.ApplicationWindow;
        }

        public static Form GetRevitWindow()
        {
            var handle = ComponentManager.ApplicationWindow;
            var window = System.Windows.Forms.Form.FromChildHandle(handle) as Form;
            return window;
        }

        public static IWin32Window GetRevitWindow_win32()
        {
            var handle = ComponentManager.ApplicationWindow;
            var window = Form.FromChildHandle(handle) as IWin32Window;
            return window;
        }
    }
}