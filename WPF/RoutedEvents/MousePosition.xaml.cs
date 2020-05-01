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
    /// MousePosition.xaml 的交互逻辑
    /// </summary>
    public partial class MousePosition : Window
    {
        public MousePosition()
        {
            InitializeComponent();
        }

        private void CmdCapture_OnClick(object sender, RoutedEventArgs e)
        {
            this.AddHandler(
                Mouse.LostMouseCaptureEvent,
                new RoutedEventHandler(this.LostCapture));
            Mouse.Capture(rect);
            CmdCapture.Content = "[ Mouse in now captured ...]";
        }

        private void MouseMoved(object sender, MouseEventArgs e)
        {
            Point pt = e.GetPosition(this);
            LblInfo.Text = $"You are at({pt.X},{pt.Y})in window coordinates";
        }

        private void LostCapture(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Lost Capture");
            CmdCapture.Content = "Capture the Mouse";
        }
    }
}