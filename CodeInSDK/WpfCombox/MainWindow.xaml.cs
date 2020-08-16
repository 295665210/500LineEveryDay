using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
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

namespace WpfCombox
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //【1】 一般绑定

            //【2】List传值
            List<string> list = new List<string>() {"1", "2", "3"};
            CbList.ItemsSource = list; //代码绑定属性。

            //【3】类传值
            List<Person> personList = new List<Person>()
            {
                new Person() {Name = "张三", Age = 15, Id = 1},
                new Person() {Name = "李四", Age = 25, Id = 2},
                new Person() {Name = "王五", Age = 35, Id = 3}
            };

            //1资源绑定
            CbClass.ItemsSource = personList; 
            //2 选择展示的对象路径
            CbClass.DisplayMemberPath = "Name";
            //3 确定选定后的值是哪个属性
            CbClass.SelectedValuePath = "Id";
            


        }


        private void CbNormal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.TxBNormal.Text = ((ComboBoxItem) CbNormal.SelectedItem).Content.ToString();
            // this.TxBNormal.Text = CbNormal.SelectedValue.ToString();  //这样是不行的。
        }


        private void CbList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.TxBList.Text = CbList.SelectedValue.ToString();
        }

        private void CbClass_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.TxBClass.Text = CbClass.SelectedValue.ToString();
        }
    }
}