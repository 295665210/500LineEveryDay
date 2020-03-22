using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using CodeInTangsengjiewa4.General.UIs;

namespace CodeInTangsengjiewa4.General
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_ViewSimultaneous : IExternalCommand
    {
        public static ElementId IdView1;
        public static ElementId IdView2;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;

            var planViewFilter = new ElementClassFilter(typeof(ViewPlan));
            var view3dFilter = new ElementClassFilter(typeof(View3D));
            var viewDraftingFilter = new ElementClassFilter(typeof(ViewDrafting));
            var logicalFilter = new LogicalOrFilter(new List<ElementFilter>()
            {
                planViewFilter, view3dFilter, viewDraftingFilter
            });
            var views = new FilteredElementCollector(doc).WhereElementIsNotElementType().WherePasses(logicalFilter)
                .Where(m => !(m as View).IsTemplate).Cast<View>().OrderBy(m => m.Title).ToList();
            //选择联动的两个窗口
            var selector = ViewSemutaneousSelector.Instance;
            selector.combo1.ItemsSource = views;
            selector.combo1.DisplayMemberPath = "Title";
            selector.combo1.SelectedIndex = 0;

            views.Remove(selector.combo1.SelectionBoxItem as View);

            selector.combo2.ItemsSource = views;
            selector.combo2.DisplayMemberPath = "Title";
            selector.combo2.SelectedIndex = 0;

            selector.ShowDialog();
            IdView1 = (selector.combo1.SelectionBoxItem as View).Id;
            IdView2 = (selector.combo2.SelectionBoxItem as View).Id;

            //激活这两个窗口,并关闭其余窗口
            uidoc.ActiveView = doc.GetElement(IdView1) as View;
            uidoc.ActiveView = doc.GetElement(IdView2) as View;
            var uiViews = uidoc.GetOpenUIViews();
            foreach (var uiView in uiViews)
            {
                if (uiView.ViewId == IdView1 || uiView.ViewId == IdView2)
                {
                    continue;
                }
                else
                {
                    uiView.Close();
                }
            }

            //平铺窗口
            var titleViewCommand = default(RevitCommandId);
            titleViewCommand = RevitCommandId.LookupPostableCommandId(PostableCommand.TileViews);
            uiapp.PostCommand(titleViewCommand);

            //在空闲事件中,同步视图
            uiapp.Idling += OnIdling;
            uiapp.ViewActivated += OnViewActivated;
            return Result.Succeeded;
        }

        private void OnViewActivated(object sender, ViewActivatedEventArgs e)
        {
            var uiapp = sender as UIApplication;
            var acview = uiapp.ActiveUIDocument.ActiveView;
            if (acview.Id != IdView1 && acview.Id != IdView2)
            {
                uiapp.Idling -= OnIdling;
            }
        }

        private void OnIdling(object sender, IdlingEventArgs e)
        {
            var uiapp = sender as UIApplication;
            var uidoc = uiapp.ActiveUIDocument;
            // var doc = uiapp.ActiveUIDocument.Document;
            // var view1 = doc.GetElement(IdView1) as View;
            // var view2 = doc.GetElement(IdView2) as View;

            var uiView1 = uidoc.GetOpenUIViews().FirstOrDefault(m => m.ViewId == IdView1);
            var uiView2 = uidoc.GetOpenUIViews().FirstOrDefault(m => m.ViewId == IdView2);

            var viewCorners = uiView1.GetZoomCorners();
            var corner1 = viewCorners.First();
            var corner2 = viewCorners.Last();

            uiView2.ZoomAndCenterRectangle(corner1, corner2);
        }
    }
}