using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RoutedEvents
{
    /// <summary>
    /// Menu.xaml 的交互逻辑
    /// </summary>
    public partial class Menu : Window
    {
        public Menu()
        {
            InitializeComponent();
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            //get the current button.
            Button cmd = (Button) e.OriginalSource;
            //create an instance of the window named
            //by the current button
            Type type = this.GetType();
            Assembly assembly = type.Assembly;
            Window win = (Window) assembly.CreateInstance(type.Namespace + "." + cmd.Content);

            //show the window 
            win.ShowDialog();
        }
    }
}