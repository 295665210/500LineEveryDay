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
    /// KeyPressEvents.xaml 的交互逻辑
    /// </summary>
    public partial class KeyPressEvents : Window
    {
        public KeyPressEvents()
        {
            InitializeComponent();
        }

        private void KeyEvent(object sender, KeyEventArgs e)
        {
            if ((bool) ChkIgnoreRepeat.IsChecked && e.IsRepeat) return;

            string message =
                //"At:" +e.Timestamp.ToString()+
                "Event: " + e.RoutedEvent + "" +
                "Key: " + e.Key;
            LstMessage.Items.Add(message);
        }

        private new void TextInput(object sender, TextCompositionEventArgs e)
        {
            string message =
                //
                "Event:" + e.RoutedEvent + " " +
                "Text: " + e.Text;
            LstMessage.Items.Add(message);
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            string message =
                "Event: " + e.RoutedEvent;
            LstMessage.Items.Add(message);
        }

        private void CmdCleat_Click(object sender, RoutedEventArgs e)
        {
            LstMessage.Items.Clear();
        }
    }
}