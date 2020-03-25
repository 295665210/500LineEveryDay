using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;


namespace CodeInTangsengjiewa4.CodeOfQian.WpfRevitFilter
{
    /// <summary>
    /// 交互逻辑
    /// </summary>
    public partial class MainWindowRevitFilter : Window
    {
        //1 注册外部事件
        // private Cmd_CreateWall CmdCreateWall = null;
        // private ExternalEvent createWallEvent = null;

        //数据源
        private List<int> selectUid = new List<int>(); //保存多选用户ID
        private List<int> allUid = new List<int>();    //保存全选用户ID
        private List<User> list = new List<User>();    //用户列表源数据


        public MainWindowRevitFilter()
        {
            InitializeComponent();
            this.DataBinding();

            //2 初始化
            // CmdCreateWall = new Cmd_CreateWall();
            // createWallEvent = ExternalEvent.Create(CmdCreateWall);
        }

        private void DataBinding()
        {
            for (int i = 0; i < 20; i++)
            {
                User user = new User()
                {
                    Category = "Category" + i,
                    Count = 0 + i
                };
                list.Add(user);
            }
            this.listView1.ItemsSource = list; //为ListView绑定数据源
        }

        // private void CreateWallBtn1_Click(object sender, RoutedEventArgs e)
        // {
        //
        //   
        //     // //3 属性传值
        //     // CmdCreateWall.WallHeight = Convert.ToDouble(this.TxBoxWallHeight1.Text);
        //     // //4 触发事件,执行命令
        //     // createWallEvent.Raise();
        // }
    }

    internal class User
    {
        public string Category { get; set; }
        public int Count { get; set; }
    }
}