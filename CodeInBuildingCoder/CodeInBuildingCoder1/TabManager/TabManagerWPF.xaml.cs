using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Autodesk.Revit.UI;
using Autodesk.Windows;

namespace CodeInBuildingCoder1.TabManager
{
    /// <summary>
    /// </summary>
    public partial class TabManagerWPF : Window
    {
        public TabManagerWPF(List<CheckBoxData> checkBoxDatas, IList<RibbonTab> revitRibbonTabs)
        {
            this.InitializeComponent();
            TabManagerWPF._checkBoxDatas = checkBoxDatas;
            this.listbox.ItemsSource = TabManagerWPF._checkBoxDatas;
            this.handler = new TabManagerWPF.EventHandler();
            this.exevent = ExternalEvent.Create(this.handler);
            TabManagerWPF._revitRibbonTabs = revitRibbonTabs;
            base.KeyDown += this.TabMamagerWPF_KeyDown;
        }

        private void TabMamagerWPF_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                base.Close();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.exevent.Raise();
            this.SaveSetting();
        }

        private void SaveSetting()
        {
            string text = "";
            for (int i = 0; i < TabManagerWPF._checkBoxDatas.Count; i++)
            {
                if (i == TabManagerWPF._checkBoxDatas.Count - 1)
                {
                    text = text + TabManagerWPF._checkBoxDatas[i].IsSelected.ToString() + "," +
                           TabManagerWPF._checkBoxDatas[i].CheckName;

                    // Settings1.Default.TabManager = text;
                    // Settings1.Default.Save();
                    return;
                }
                text = string.Concat(new string[]
                {
                    text,
                    TabManagerWPF._checkBoxDatas[i].IsSelected.ToString(),
                    ",",
                    TabManagerWPF._checkBoxDatas[i].CheckName,
                    ";"
                });
            }
        }

        private static List<CheckBoxData> _checkBoxDatas;

        private ExternalEvent exevent;

        private IExternalEventHandler handler;

        private static IList<RibbonTab> _revitRibbonTabs;

        public class EventHandler : IExternalEventHandler
        {
            public void Execute(UIApplication app)
            {
                IList<RibbonTab> revitRibbonTabs = TabManagerWPF._revitRibbonTabs;
                foreach (CheckBoxData checkBoxData in TabManagerWPF._checkBoxDatas)
                {
                    foreach (RibbonTab ribbonTab in revitRibbonTabs)
                    {
                        if (checkBoxData.CheckName == "快速弹夹")
                        {
                            checkBoxData.IsSelected = true;
                        }
                        else if (ribbonTab.AutomationName == checkBoxData.CheckName)
                        {
                            if (checkBoxData.IsSelected)
                            {
                                ribbonTab.IsVisible = true;
                            }
                            else
                            {
                                ribbonTab.IsVisible = false;
                            }
                        }
                    }
                }
            }

            // Token: 0x0600013B RID: 315 RVA: 0x00002B89 File Offset: 0x00000D89
            public string GetName()
            {
                return "text";
            }
        }
    }
}