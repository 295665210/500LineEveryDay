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
using Autodesk.Revit.DB;

namespace CreateFilters
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class CreateFiltersMainWindow : Window
    {
        public static List<string> dtSystems = new List<string>()
        {
            "排烟", "排风", "排风兼排烟", "补风", "送风兼补风", "正压送风", "新风", "回风", "送风", "排油烟"
        };

        public static List<string> pisystems = new List<string>()
        {
            "给水", "热水", "污水", "废水", "中水", "雨水", "消防", "喷淋", "水幕", "水炮", "气体灭火", "冷冻供水",
            "冷冻回水", "冷却供水", "冷却回水", "冷凝水", "冷媒管", "空调补水", "供暖", "燃气"
        };

        public static List<string> ctsystems = new List<string> {"强电", "弱电"};

        public List<string> stNames = new List<string>();

        public ElementId paraId = null;

        public List<ElementId> categoryId = new List<ElementId>();


        public CreateFiltersMainWindow()
        {
            InitializeComponent();
        }


        private void Dt_Click(object sender, RoutedEventArgs e)
        {
            if (categoryId.Count != 0)
            {
                categoryId.Clear();
                categoryId.Add(new ElementId(BuiltInCategory.OST_DuctCurves));
            }
            else
            {
                categoryId.Add(new ElementId(BuiltInCategory.OST_DuctCurves));
            }

            paraId = new ElementId(BuiltInParameter.RBS_DUCT_SYSTEM_TYPE_PARAM);
            DeleteOldCheckBox();
            CreateSystemTypesList(dtSystems);
        }

        /// <summary>
        /// 根据系统名称生成列表
        /// </summary>
        /// <param name="mepsystems"></param>
        private void CreateSystemTypesList(List<string> mepsystems)
        {
            for (int i = 0; i < mepsystems.Count; i++)
            {
                CheckBox cb = new CheckBox()
                {
                    Content = mepsystems[i], FontSize = 16
                };
                systemTypes.Children.Add(cb);
            }
        }

        private void DeleteOldCheckBox()
        {
            if (systemTypes.Children.Count != 0)
            {
                CheckBox[] cbs = new CheckBox[systemTypes.Children.Count];
                systemTypes.Children.CopyTo(cbs, 0);
                foreach (var cb in cbs)
                {
                    systemTypes.Children.Remove(cb);
                }
            }
        }


        private void Comfirm_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in systemTypes.Children)
            {
                if (item is CheckBox)
                {
                    stNames.Add(((CheckBox) item).Content.ToString());
                }
            }
            this.DialogResult = true;
        }

        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in systemTypes.Children)
            {
                if (item is CheckBox)
                {
                    ((CheckBox)item).IsChecked = true;
                }
            }
        }

        private void Pi_Click(object sender, RoutedEventArgs e)
        {
            if (categoryId.Count != 0)
            {
                categoryId.Clear();
                categoryId.Add(new ElementId(BuiltInCategory.OST_PipeCurves));
            }
            else
            {
                categoryId.Add(new ElementId(BuiltInCategory.OST_PipeCurves));
            }
            paraId = new ElementId(BuiltInParameter.RBS_SYSTEM_CLASSIFICATION_PARAM);
            DeleteOldCheckBox();
            CreateSystemTypesList(pisystems);
        }

        private void Ct_Click(object sender, RoutedEventArgs e)
        {
            DeleteOldCheckBox();
            CreateSystemTypesList(ctsystems);
        }

        private void NotselectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in systemTypes.Children)
            {
                if (item is CheckBox)
                {
                    ((CheckBox)item).IsChecked = false;
                }
            }
        }
    }
}