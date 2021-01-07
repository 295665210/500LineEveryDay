using RevitStorage.StructuredStorage;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using MessageBox = System.Windows.MessageBox;

namespace FamilyManagerUI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 调用windows自身库，释放图片资源
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteObject(IntPtr value);

        /// <summary>
        /// 定义初始变量
        /// </summary>
        private int numberOfRecPerPage; //每页的个数
        private Paging pagedTable = new Paging(); //使用分页函数
        private List<FamilyObject> myList = new List<FamilyObject>(); //初始化目标族对象
        private List<string> fileList = new List<string>(); //初始化族文件列表
        public MainWindow()
        {
            InitializeComponent();
            pagedTable.PageIndex = 0;
            int[] RecordsToShow = { 10, 20, 30, 50, 100 }; //设置每页显示个数值

            foreach (int RecordGroup in RecordsToShow)
            {
                NumberOfRecords.Items.Add(RecordGroup); //填充下拉页面值
            }
        }

        #region 基本按钮事件
        /// <summary>
        /// 上一个
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Backwards_Click(object sender, RoutedEventArgs e)
        {
            ListViewItems.ItemsSource = pagedTable.Previous(myList, numberOfRecPerPage);
            CurrentText.Text = PageNumberDisplay();
        }
        /// <summary>
        /// 下一个
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Forward_Click(object sender, RoutedEventArgs e)   
        {                                                              
            ListViewItems.ItemsSource = pagedTable.Next(myList, numberOfRecPerPage);
            CurrentText.Text = PageNumberDisplay();
        }
        
        /// <summary>
        /// 第一个
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void First_Click(object sender, RoutedEventArgs e)
        {
            ListViewItems.ItemsSource = pagedTable.First(myList, numberOfRecPerPage);
            CurrentText.Text = PageNumberDisplay();
        }

        /// <summary>
        /// 最后一个
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Last_Click(object sender, RoutedEventArgs e)
        {
            ListViewItems.ItemsSource = pagedTable.Last(myList, numberOfRecPerPage);
            CurrentText.Text = PageNumberDisplay();
        }

        /// <summary>
        /// 当下拉页面变化时，触发新的分页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NumberOfRecords_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            numberOfRecPerPage = Convert.ToInt32(NumberOfRecords.SelectedItem);
            ListViewItems.ItemsSource = pagedTable.First(myList, numberOfRecPerPage);
            CurrentText.Text = PageNumberDisplay();
        }

        /// <summary>
        /// 打开文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenBtn_Click(object sender, RoutedEventArgs e)
        {

            //先清空已有的文件list，和族list
            myList.Clear();
            fileList.Clear();
            this.SearchText.Clear();
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.Description = "请选择一个文件夹路径";//对话框名称
            dlg.SelectedPath = @"E:\课程录制\Revit二次开发进阶课程\测试文件\族文件\机械设备";//默认路径
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TextPath.Text = dlg.SelectedPath;
            }
            else
            {
                return;
            }
            //添加每一个路径到族文件路径中
            foreach (var item in Directory.GetFiles(dlg.SelectedPath))
            {
                if (item.Contains(".rfa"))
                {
                    fileList.Add(item);
                }
            }

            if (fileList.Count > 0)
            {
                myList = GetFamilyObjects(fileList);
            }
            else
            {
                return;
            }
            InitPage();
        }

        /// <summary>
        /// 筛选族文件,根据关键词查找,如果不填关键词，则默认查找全部
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SerachBtn_Click(object sender, RoutedEventArgs e)
        {
            //得到输入的关键词
            string seachWord = this.SearchText.Text.Trim();
            myList = GetFamilyObjects(fileList);
            //如果没有填，则初始化
            if (string.IsNullOrWhiteSpace(seachWord))
            {
                InitPage();
                return;
            }
            else
            {
                myList = new List<FamilyObject>(myList.FindAll(x => x.Name.Contains(seachWord)));
            }

            if (myList.Count > 0)
            {
                InitPage();
            }
            else
            {
                ListViewItems.ItemsSource = null;
                CurrentText.Text = "没有此族！";

            }
        }


        /// <summary>
        /// 载入模型
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadBtn_Click(object sender, RoutedEventArgs e)
        {
            FamilyObject data = ((System.Windows.Controls.Button)sender).Tag as FamilyObject;
            SysCache.Instance.CurrentRfaLocation = data.Locatoin;
            SysCache.Instance.LoadEvent.Raise();
            MessageBox.Show($"{ SysCache.Instance.CurrentRfaLocation}", "准备载入");
        }
        #endregion

        #region 功能小函数

        /// <summary>
        /// 根据当前myList初始化页面
        /// </summary>
        private void InitPage()
        {
            //开始在前台显示内容
            ListViewItems.ItemsSource=null;  //清空当前页面
            pagedTable.PageIndex = 0; //设置初始页面
            NumberOfRecords.SelectedItem = 10; //初始化页面展示个数10
            numberOfRecPerPage = Convert.ToInt32(NumberOfRecords.SelectedItem);                                                         //得到当前页面展示的对象
            List<FamilyObject> currentFamilyObjects = pagedTable.SetPaging(myList, numberOfRecPerPage);//得到分页后当前页面的对象
            if (currentFamilyObjects.Count > 0)
                ListViewItems.ItemsSource = currentFamilyObjects;
            CurrentText.Text = PageNumberDisplay();//展示当前显示内容
        }
        /// <summary>
        /// 通过文件夹下的族获取模型目标列表
        /// </summary>
        /// <param name="fileList"></param>
        /// <returns></returns>
        private List<FamilyObject> GetFamilyObjects(List<string> fileList)
        {
            //清空之前的文件列表
            myList.Clear();
            //将每个一符合的族对象添加到目标族列表中
            foreach (var item in fileList)
            {
                //通过不打开族的方式获取族的预览图片
                Storage storage = new Storage(item);
                
                BitmapSource bms = GetImageStream(storage.ThumbnailImage.GetPreviewAsImage());
                myList.Add(new FamilyObject()
                {
                    //Path:E:\课程录制\Revit二次开发进阶课程\测试文件\族文件\机械设备\多联机 - 室内机 - 双向气流 - 天花板嵌入式.rfa
                    Name = Path.GetFileNameWithoutExtension(item),//多联机 - 室内机 - 双向气流 - 天花板嵌入式
                    //  Path.GetFileName()  多联机 - 室内机 - 双向气流 - 天花板嵌入式.rfa
                    Locatoin = item,
                    RfaImage = bms
                });
            }
            return myList;
        }

        /// <summary>
        /// 图片格式从System.Windows.Image转为BitmapSource
        /// 这里是为了变成Xmal中System.Windows.Controls.Image控件的source绑定
        /// 这里就用到了资源的释放
        /// </summary>
        /// <param name="myImage"></param>
        /// <returns></returns>
        public BitmapSource GetImageStream(System.Drawing.Image myImage)
        {
            var bitmap = new Bitmap(myImage);
            IntPtr bmpPt = bitmap.GetHbitmap();
            BitmapSource bitmapSource =
             System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                   bmpPt,
                   IntPtr.Zero,
                   Int32Rect.Empty,
                   BitmapSizeOptions.FromEmptyOptions());

            //释放资源
            bitmapSource.Freeze();
            DeleteObject(bmpPt);

            return bitmapSource;
        }
      

        /// <summary>
        /// 得到当前页面显示状态
        /// </summary>
        /// <returns>string Number of Records Showing</returns>
        public string PageNumberDisplay()
        {
            int PagedNumber = numberOfRecPerPage * (pagedTable.PageIndex + 1);
            if (PagedNumber > myList.Count)
            {
                PagedNumber = myList.Count;
            }
            return "当前显示：" + PagedNumber + "/" + myList.Count; //This dramatically reduced the number of times I had to write this string statement
        }

        #endregion

   
    }
}
