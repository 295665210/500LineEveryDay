using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CodeInTangsengjiewa4.CodeInFunny.FamilyLibrary
{
    class PreviewExEvent : IExternalEventHandler
    {
        public string familyPath = string.Empty;
        public Document doc = null;
        private Application _application = null;
        private ElementId elementId;
        public List<DBViewItem> famViews;

        public void Execute(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            _application = uiapp.Application;
            doc = _application.OpenDocumentFile(familyPath);
            try
            {
                UpdateViewsList(doc);
            }
            catch (Exception e)
            {
                TaskDialog.Show("Error", e.Message);
            }
        }

        /// <summary>
        /// 更新视图列表
        /// </summary>
        /// <param name="doc"></param>
        private void UpdateViewsList(Document doc)
        {
            //过滤当前文档可以查看的视图
            FilteredElementCollector viewsFc = new FilteredElementCollector(doc).OfClass(typeof(View));
            IEnumerable<View> _dbViews = from Element x in viewsFc
                                         where (x as View).CanBePrinted == true
                                         select x as View;
            // if (Commands.mainWindow.viewCB.Items.Count != 0)
            // {
            //     Commands.mainWindow.viewCB.Items.Clear();
            // }

            famViews = new List<DBViewItem>();
            DBViewItem activeItem = null;
            bool isEmpty = true;
            foreach (var view in _dbViews)
            {
                elementId = view.Id;
                isEmpty = false;
                activeItem = new DBViewItem(view, doc);
                famViews.Add(activeItem);
                // Commands.mainWindow.viewCB.Items.Add(activeItem);
            }

            if (isEmpty == true)
            {
                View3D view = View3D.CreateIsometric(doc, elementId);
                activeItem = new DBViewItem(view, doc);
                // Commands.mainWindow.viewCB.Items.Add(activeItem);
                famViews.Add(activeItem);
            }

            Commands.mainWindow.viewCB.ItemsSource = famViews;
            Commands.mainWindow.viewCB.SelectedItem = activeItem;
        }

        public string GetName()
        {
            return "预览族模型";
        }
    }
}