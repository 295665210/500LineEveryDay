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

namespace EightBalls
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

        private void BtnAnswer_OnClick(object sender, RoutedEventArgs e)
        {
            //dramatic delay...
            this.Cursor = Cursors.Wait;//鼠标样式变为等待
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));//挂起线程一定的时间.

            AnswerGenerator generator =new AnswerGenerator();
            AnswerTBox.Text = generator.GetRandomAnswer(TxtQuestion.Text);
            this.Cursor = null;//改变鼠标样式.
        }
    }
}
