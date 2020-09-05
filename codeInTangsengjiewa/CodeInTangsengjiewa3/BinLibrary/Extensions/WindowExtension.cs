using System.Windows;
using System.Windows.Interop;

namespace CodeInTangsengjiewa3.BinLibrary.Extensions
{
    public static class WindowExtension
    {
        public static WindowInteropHelper helper(this Window win)
        {
            var helper = new WindowInteropHelper(win);
            return helper;
        }
    }
}