using System;
using System.Collections.Generic;
using System.Linq;
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
    /// BubbledLabelClick.xaml 的交互逻辑
    /// </summary>
    public partial class BubbledLabelClick : Window
    {
        public BubbledLabelClick()
        {
            InitializeComponent();
        }

        protected int EventCounter = 0;

        private void SomethingClicked(object sender, MouseButtonEventArgs e)
        {
            EventCounter++;
            string message = "#" + EventCounter.ToString() + ":\r\n" +
                "sender:  " + sender.ToString() + "\r\n" +
                "source:  " + e.Source + "\r\n" +
                "original source:  " + e.OriginalSource;

            lstMessage.Items.Add(message);
            e.Handled = (bool) chkHandle.IsChecked;
        }

        private void cmdClear_Click(object sender, RoutedEventArgs e)
        {
            EventCounter = 0;
            lstMessage.Items.Clear();
        }
    }
}