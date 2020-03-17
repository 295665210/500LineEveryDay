using Autodesk.Revit.UI;
using System;
using System.Windows;


namespace CodeInTangsengjiewa4.CodeOfQian.WpfEventDemo
{
    /// <summary>
    /// 交互逻辑
    /// </summary>
    public partial class MainWindowCreateWall : Window
    {
        //1 注册外部事件
        private Cmd_CreateWall CmdCreateWall = null;
        private ExternalEvent createWallEvent = null;


        public MainWindowCreateWall()
        {
            InitializeComponent();
            //2 初始化
            CmdCreateWall = new Cmd_CreateWall();
            createWallEvent = ExternalEvent.Create(CmdCreateWall);
        }

        private void CreateWallBtn1_Click(object sender, RoutedEventArgs e)
        {

          
            //3 属性传值
            CmdCreateWall.WallHeight = Convert.ToDouble(this.TxBoxWallHeight1.Text);
            //4 触发事件,执行命令
            createWallEvent.Raise();
        }
    }
}