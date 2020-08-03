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
using Singleton2HuanCun;

namespace Singleton
{
    /// <summary>
    /// ChildWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ChildWindow : Window
    {
        //改造为单例模式
        //1 public 改为 private， 就不能用new方法创建
        private ChildWindow()
        {
            InitializeComponent();
        }

        //
        private static ChildWindow _instance;
        public static ChildWindow Instance
        {
            get
            {
                if (ReferenceEquals(_instance, null))
                {
                    _instance = new ChildWindow();
                }
                return _instance;
            }
        }

        private void BtnShow_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(SysCache.Instance.TextValue, "主窗体的值：");
        }
    }
}