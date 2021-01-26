using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Windows;
using UIFramework;

namespace CodeInBuildingCoder1.TabManager
{
    [Transaction(TransactionMode.Manual)]
    class CMDTabManager : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            bool isFamilyDocument = commandData.Application.ActiveUIDocument.Document.IsFamilyDocument;
            List<CheckBoxData> list = new List<CheckBoxData>();
            List<string> list2 = new List<string>();
            string text = "";
            IEnumerable<RibbonTab> tabs = RevitRibbonControl.RibbonControl.Tabs;
            IList<RibbonTab> list3 = new List<RibbonTab>();
            foreach (RibbonTab ribbonTab in tabs)
            {
                text = string.Concat(new string[] {text, ribbonTab.AutomationName, ",,,", ribbonTab.Id, "\n"});
                if (isFamilyDocument)
                {
                    if ((ribbonTab.Id.Contains("_") || ribbonTab.Id.Contains("Family")) &&
                        !list2.Contains(ribbonTab.AutomationName))
                    {
                        CheckBoxData item = new CheckBoxData(ribbonTab.IsVisible, ribbonTab.AutomationName);
                        list.Add(item);
                        list2.Add(ribbonTab.AutomationName);
                        list3.Add(ribbonTab);
                    }
                }
                else if (!ribbonTab.Id.Contains("_") && !ribbonTab.Id.Contains("Family"))
                {
                    if (!list2.Contains(ribbonTab.AutomationName))
                    {
                        CheckBoxData item2 = new CheckBoxData(ribbonTab.IsVisible, ribbonTab.AutomationName);
                        list.Add(item2);
                        list2.Add(ribbonTab.AutomationName);
                        list3.Add(ribbonTab);
                    }
                }
                else
                {
                    ribbonTab.IsVisible = false;
                }
            }
            TabManagerWPF tabManagerWpf = new TabManagerWPF(list, list3);
            new WindowInteropHelper(tabManagerWpf).Owner = Process.GetCurrentProcess().MainWindowHandle;
            tabManagerWpf.Show();
            return Result.Succeeded;
        }
    }
}