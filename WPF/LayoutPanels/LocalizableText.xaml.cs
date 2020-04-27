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
    public partial class LocalizableText : Window
    {
        public LocalizableText()
        {
            InitializeComponent();
        }


        private void ChkLongText_OnUnchecked(object sender, RoutedEventArgs e)
        {
            cmdPrev.Content = "<- Go to the Previous Window";
            cmdNext.Content = "Go to the Next Window ->";
        }

        private void ChkLongText_OnChecked(object sender, RoutedEventArgs e)
        {
            cmdPrev.Content = "Prev";
            cmdNext.Content = "Next";
        }
    }
}