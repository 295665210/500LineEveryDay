using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FConsoleMain
{
    class P19HiThere
    {
        [STAThread]
        static void Main(string[] args)
        {
            Window myWin = new Window();
            myWin.Title = "My simple window";
            myWin.Content = "Hi there";

            Application myApp = new Application();
            myApp.Run(myWin);
        }
    }
}