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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Singleton2HuanCun;

namespace Singleton
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

        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            // ChildWindow childWindow = new ChildWindow();

            //子窗口修改为单例模式后：
            // ChildWindow childWindow = ChildWindow.Instance;
            // childWindow.Show();

            //简化为一行
            ChildWindow.Instance.Show();
            
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //通过缓存，实现父窗体与子窗体传值。类似全局变量。 
            SysCache.Instance.TextValue = TextBox.Text.Trim();
        }

        /// <summary>
        /// 程序关闭时，清理缓存。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}