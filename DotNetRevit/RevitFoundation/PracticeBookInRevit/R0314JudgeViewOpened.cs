using Autodesk.Revit.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitFoundation.PracticeBookInRevit.UIs;

namespace RevitFoundation.PracticeBookInRevit
{
    /// <summary>
    /// 2020年3月14日 作业:
    /// 1.判断某一个视图是否被打开。
    /// 3.获取当前的uiview
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class R0314JudgeViewOpened : IExternalCommand
    {
        public static ElementId viewSelected;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var app = uiapp.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;

            //1. 判断某一个视图是否被打开。
            //获得所有的视图集合
            var planViewFilter = new ElementClassFilter(typeof(ViewPlan));
            var view3dFilter = new ElementClassFilter(typeof(View3D));
            var viewDraftingFilter = new ElementClassFilter(typeof(ViewDrafting));

            var logicalOrFilter = new LogicalOrFilter(new List<ElementFilter>()
                                                          {planViewFilter, view3dFilter, viewDraftingFilter});
            var views = new FilteredElementCollector(doc).WhereElementIsNotElementType().WherePasses(logicalOrFilter)
                .Where(m => !(m as View).IsTemplate).Cast<View>().OrderBy(m => m.Title).ToList();
            // string info = "";
            // foreach (var view in views)
            // {
            //     info += view.Title + "\n";
            // }
            // MessageBox.Show(info);
            var selector = ViewSelector.Instance;
            selector.combo1.ItemsSource = views;
            selector.combo1.DisplayMemberPath = "Title";
            selector.combo1.SelectedIndex = 0;
            selector.ShowDialog();
            viewSelected = (selector.combo1.SelectionBoxItem as View).Id;

            var targetUIView = uidoc.GetOpenUIViews().Where(m => m.ViewId == viewSelected).ToList();
            if (targetUIView.Count == 0)
            {
                MessageBox.Show("选择视图 没有 打开");
                return Result.Succeeded;
            }
            else if (targetUIView.Count == 1)
            {
                MessageBox.Show("选择视图已经打开");
                return Result.Succeeded;
            }
            else
            {
                return Result.Cancelled;
            }
        }
    }
}