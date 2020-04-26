using System;
using System.Collections.Generic;
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

namespace LayoutPanels
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SimpleStack : Window
    {
        public SimpleStack()
        {
            InitializeComponent();
        }


        private void ChkVertical_Checked(object sender, RoutedEventArgs e)
        {
            StackPanel1.Orientation = Orientation.Horizontal;
        }

        private void ChkVertical_Unchecked(object sender, RoutedEventArgs e)
        {
            StackPanel1.Orientation = Orientation.Vertical;
        }
    }
}