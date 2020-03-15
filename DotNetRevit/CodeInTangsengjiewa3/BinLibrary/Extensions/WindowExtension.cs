using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace CodeInTangsengjiewa3.BinLibrary.Extensions
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