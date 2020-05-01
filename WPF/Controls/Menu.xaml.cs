using System;
using System.Collections.Generic;
using System.Design;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Controls
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
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