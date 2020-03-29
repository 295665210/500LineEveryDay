using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RApplication = Autodesk.Revit.ApplicationServices.Application;
using DBView = Autodesk.Revit.DB.View;
using CodeInTangsengjiewa4.CodeInFunny.FamilyLibrary;
using MessageBox = System.Windows.MessageBox;
using System;
using TreeView = System.Windows.Controls.TreeView;

namespace WpfApps
{
    /// <summary>
    /// PreviewModel.xaml 的交互逻辑
    /// </summary>
    public partial class PreviewModel : Window
    {
        private Document doc = null;
        private Document famDoc = null;
        private RApplication app = null;
        private ElementId viewId = null;
        private Transaction trans = null;
        private DBViewItem dbItem = null;
        private StartConfig sc = new StartConfig();
        private FileInfo familyInfo = null;
        private bool tag = false;

        public PreviewControl pc = null;
        DirectoryInfo dirInfo = null;
        private string familyFilePath = string.Empty;
        private List<DBViewItem> dbViewsList = null;

        List<string> dlName = new List<string>() {"粗细", "中等", "粗略"};
        List<string> dsName = new List<string>() {"真实", "着色", "隐藏线", "线框"};

        public PreviewModel(RApplication rapp, UIApplication uiapp)
        {
            InitializeComponent();
            this.app = rapp;
            this.doc = uiapp.ActiveUIDocument.Document;
        }

        private void ViewCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Windows.Controls.ComboBox cb = (System.Windows.Controls.ComboBox) sender;
            if (cb == null)
            {
                return;
            }
            dbItem = cb.SelectedItem as DBViewItem;

            if (dbItem == null)
            {
                return;
            }

            if (pc != null)
            {
                pc.Dispose();
            }

            trans = new Transaction(famDoc, "视图设置");
            trans.Start();
            if (dbItem.View.HasDetailLevel())
            {
                dbItem.View.DetailLevel = ViewDetailLevel.Fine;
            }

            if (dbItem.View.HasDisplayStyle())
            {
                dbItem.View.DisplayStyle = DisplayStyle.Realistic;
            }

            trans.Commit();
            pc = new PreviewControl(famDoc, dbItem.Id);
            preView.Children.Add(pc);
            deLevel.SelectedItem = "精细";
            dsStyle.SelectedItem = "真实";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Directory.Exists(sc.StartPath))
                {
                    FolderBrowserDialog fbDialog = new FolderBrowserDialog();
                    DialogResult result = fbDialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.Cancel)
                    {
                        tag = true;
                    }
                    if (tag == false)
                    {
                        sc.StartPath = fbDialog.SelectedPath.Trim();
                        sc.Save();
                    }
                }
                if (tag)
                {
                    Close();
                }
                else
                {
                    dirInfo = new DirectoryInfo(sc.StartPath);

                    upLoadBTN.IsEnabled = false;
                    loadBTN.IsEnabled = false;
                }
            }
            catch (Exception)
            {
                tag = true;
            }
        }

        private void FamilyTree_Loaded(object sender, RoutedEventArgs e)
        {
            //遍历文件夹
            if (tag)
            {
                return;
            }

            if (dirInfo.GetDirectories().Count() != 0)
            {
                foreach (DirectoryInfo di in dirInfo.GetDirectories()) //算法有问题??
                {
                    TreeViewItem item = new TreeViewItem();
                    item.Tag = di;
                    item.Header = di.Name;
                    //如果当前子目录下有子目录的话,或者族文件的话,添加子项
                    if (di.GetDirectories().Length > 0 || di.GetFiles("*.rfa").Length > 0)
                    {
                        item.Items.Add("*");
                        //添加子项
                        familyTree.Items.Add(item);
                        //遍历族文件
                        CreateFamilyItems(dirInfo, item);
                    }
                }
            }
            else
            {
                if (dirInfo.GetFiles("*.rfa").Count() == 0)
                {
                    return;
                }

                foreach (FileInfo fi in dirInfo.GetFiles("*.rfa"))
                {
                    TreeViewItem item = new TreeViewItem();
                    item.Tag = fi;
                    item.Header = fi.Name;
                    familyTree.Items.Add(item);
                }
            }
        }

        private void FamilyTree_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem) e.OriginalSource;
            item.Items.Clear();
            //遍历文件夹
            DirectoryInfo di = (DirectoryInfo) item.Tag;
            foreach (var subDi in di.GetDirectories())
            {
                //创建子项
                TreeViewItem subItem = new TreeViewItem();
                subItem.Tag = subDi;
                subItem.Header = subDi.Name;

                if (subDi.GetDirectories().Length > -0 || subDi.GetFiles("*.rfa").Length > 0)
                {
                    subItem.Items.Add("*");
                    item.Items.Add(subItem);
                }
            }
            //遍历族文件
            CreateFamilyItems(di, item);
        }

        //节点选中事件
        private void FamilyTree_Selected(object sender, RoutedEventArgs e)
        {
            try
            {
                if (preView.Children != null)
                {
                    preView.Children.Clear();
                }
                TreeViewItem item = (TreeViewItem) e.Source;
                if (item.Tag is FileInfo)
                {
                    loadBTN.IsEnabled = true;
                    upLoadBTN.IsEnabled = true;
                    familyInfo = item.Tag as FileInfo;
                    familyFilePath = familyInfo.FullName;
                    //prEx.familypath = familyFilePath;
                    //ExternalEventRequest ex = prEvent.Raise();
                    dsStyle.ItemsSource = dsName;
                    deLevel.ItemsSource = dlName;
                    famDoc = app.OpenDocumentFile(familyFilePath);
                    UpdataPreviews(famDoc);
                }
                else
                {
                    upLoadBTN.IsEnabled = false;
                    loadBTN.IsEnabled = false;
                }
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message);
            }
        }

        //选择路径
        private void SelectFilePath_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.Cancel)
                {
                    return;
                }

                if (familyTree.Items.Count != 0)
                {
                    familyTree.Items.Clear();
                }

                sc.StartPath = dialog.SelectedPath.Trim(); //选择路径并存储路径
                sc.Save();
                dirInfo = new DirectoryInfo(sc.StartPath);
                FamilyTree_Loaded(sender, e);
            }
            catch (Exception) //如果发生异常,重设路径
            {
                sc.Reset();
            }
        }

        //加载族
        private void LoadBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (familyTree.SelectedItem != null)
                {
                    TreeViewItem item = familyTree.SelectedItem as TreeViewItem;
                    familyInfo = item.Tag as FileInfo;
                    familyFilePath = familyInfo.FullName;
                    //  lfEx.filePath = familyFilePath;
                    // lfEvent.Raise();
                    // if (lfEx.result)
                    using (trans = new Transaction(doc, "加载族"))
                    {
                        trans.Start();
                        if (doc.LoadFamily(familyFilePath))
                        {
                            Close();
                            MessageBox.Show("载入成功");
                        }
                    }
                }
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message);
            }
        }

        /// <summary>
        /// 创建族列表
        /// </summary>
        /// <param name="dirInfo"></param>
        /// <param name="control"></param>
        private void CreateFamilyItems(DirectoryInfo dirInfo, System.Windows.Controls.Control control)
        {
            foreach (FileInfo fi in dirInfo.GetFiles("*.rfa"))
            {
                TreeViewItem item = new TreeViewItem();
                item.Tag = fi;
                item.Header = fi.Name;
                if (control is System.Windows.Controls.TreeView)
                {
                    ((TreeView) control).Items.Add(item);
                    continue;
                }
                if (control is TreeViewItem)
                {
                    ((TreeViewItem) control).Items.Add(item);
                }
            }
        }


        //视图精度设置
        private void DeLevle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dbItem == null)
            {
                return;
            }
            if (preView.Children != null)
            {
                preView.Children.Clear();
            }
            if (pc != null)
            {
                pc.Dispose();
            }

            using (trans = new Transaction(famDoc, "设置显示精度"))
            {
                if (dbItem.View.HasDetailLevel())
                {
                    trans.Start();
                    switch (deLevel.SelectedItem.ToString())
                    {
                    case "精细":
                        dbItem.View.DetailLevel = ViewDetailLevel.Fine;
                        break;
                    case "中等":
                        dbItem.View.DetailLevel = ViewDetailLevel.Medium;
                        break;
                    case "粗略":
                        dbItem.View.DetailLevel = ViewDetailLevel.Coarse;
                        break;
                    }
                    trans.Commit();
                }
                else
                {
                    return;
                }
            }
            pc = new PreviewControl(famDoc, dbItem.Id);
            preView.Children.Add(pc);
        }

        //显示模式设置
        private void DsStyle_selectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dbItem == null)
            {
                return;
            }
            if (preView.Children != null)
            {
                preView.Children.Clear();
            }
            if (pc != null)
            {
                pc.Dispose();
            }
            using (trans = new Transaction(famDoc, "设置显示类型"))
            {
                if (dbItem.View.HasDisplayStyle())
                {
                    trans.Start();
                    switch (dsStyle.SelectedItem.ToString())
                    {
                    case "真实":
                        dbItem.View.DisplayStyle = DisplayStyle.Realistic;
                        break;
                    case "着色":
                        dbItem.View.DisplayStyle = DisplayStyle.FlatColors;
                        break;
                    case "隐藏线":
                        dbItem.View.DisplayStyle = DisplayStyle.HLR;
                        break;
                    case "线框":
                        dbItem.View.DisplayStyle = DisplayStyle.Wireframe;
                        break;
                    }
                    trans.Commit();
                }
                else
                {
                    return;
                }
                pc = new PreviewControl(famDoc, dbItem.Id);
                preView.Children.Add(pc);
            }
        }


        private void UpLoadBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog _openFileDialog = new OpenFileDialog();
                _openFileDialog.Filter = "族文件 (*.rfa)|*.rfa"; //过滤文件
                DialogResult result = _openFileDialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.Cancel)
                {
                    return;
                }

                string destDir = familyInfo.DirectoryName;      //当前文件夹路径
                string destFileName = _openFileDialog.FileName; //当前文件的完整路径
                string destFile = Path.Combine(destDir, Path.GetFileName(destFileName));
                if (Path.HasExtension(destFileName))
                {
                    if (Path.GetExtension(destFileName) != "*.rfa")
                    {
                        MessageBox.Show("请选择Revit族文件");
                        return;
                    }
                    File.Copy(destFileName, destFile, true);
                    dirInfo = new DirectoryInfo(sc.StartPath);
                    if (familyTree.Items.Count != 0)
                    {
                        familyTree.Items.Clear();
                    }
                    MessageBox.Show("上传成功");
                    FamilyTree_Loaded(sender, e);
                }
                else
                {
                    MessageBox.Show("请选择有效的族文件");
                }
            }
            catch (Exception s)
            {
                MessageBox.Show(s.Message);
            }
        }

        /// <summary>
        /// 更新预览视图
        /// </summary>
        /// <param name="faDoc"></param>
        private void UpdataPreviews(Document faDoc)
        {
            var viewsFc = new FilteredElementCollector(famDoc).OfClass(typeof(DBView))
                .Where(x => (x as DBView).CanBePrinted == true).ToList().ConvertAll(x => x as DBView);
            dbViewsList = new List<DBViewItem>();
            DBViewItem activeItem = null;
            bool isEmpty = true;

            foreach (var view in viewsFc)
            {
                isEmpty = false;
                viewId = view.Id;
                activeItem = new DBViewItem(view, faDoc);
                dbViewsList.Add(activeItem);
            }

            if (isEmpty)
            {
                View3D _view3D = View3D.CreateIsometric(famDoc, viewId);
                activeItem = new DBViewItem(_view3D, famDoc);
                dbViewsList.Add(activeItem);
            }
            viewCB.ItemsSource = dbViewsList;
            viewCB.SelectedItem = activeItem;
        }
    }
}