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

namespace WpfDataGrid
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //
            List<Person> personList = new List<Person>()
            {
                new Person() {Name = "张三", Age = 15, Id = 1},
                new Person() {Name = "李四", Age = 25, Id = 2},
                new Person() {Name = "王五", Age = 35, Id = 3}
            };
            //
            dgSimple.ItemsSource = personList;

            //
            dgCustom.AutoGenerateColumns = false; //自定义列时，要设置为false。
            dgCustom.ItemsSource = personList;
        }

        private void btnShowAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in dgCustom.Items)
            {
                if (item is Person person)
                {
                    string text = person.Name;
                    MessageBox.Show(text, "每一个值：");
                }
            }
        }

        private void BtnShowChoosed_Click(object sender, RoutedEventArgs e)
        {
            if (dgCustom.SelectedItem == null)
            {
                MessageBox.Show("请选择内容");
                return;
            }
            Person person = dgCustom.SelectedItem as Person;
            string text = person.Name;
            MessageBox.Show(text, "选中的值");
        }
    }
}