using System.Windows;
using System.Windows.Interop;

namespace CodeInTangsengjiewa4.BinLibrary.Extensions
{
    public static class WindowExtension
    {
        public static WindowInteropHelper Helper(this Window win)
        {
            var helper = new WindowInteropHelper(win);
            return helper;
        }
    }
}