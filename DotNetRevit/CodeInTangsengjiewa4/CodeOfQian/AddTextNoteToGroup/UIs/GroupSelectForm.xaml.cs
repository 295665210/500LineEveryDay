using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Autodesk.Revit.DB;
using CodeInTangsengjiewa4.CodeOfQian.AddTextNoteToGroup;

namespace CodeInTangsengjiewa4.CodeOfQian.AddTextNoteToGroup.UIs
{
    /// <summary>
    /// GroupSelectForm.xaml 的交互逻辑
    /// </summary>
    public partial class GroupSelectForm : Window
    {
        private List<Group> m_SelectedGroups;
        private Document m_Doc;
        private List<Group> m_UnSelectedGroups;

        internal List<Group> selectedGroups
        {
            get
            {
                return this.m_SelectedGroups;
            }
        }

        public GroupSelectForm()
        {
            InitializeComponent();
        }

        public GroupSelectForm(List<Group> groups, View view)
        {
            InitializeComponent();

            bool flag = groups.Count != 0;
            if (flag)
            {
                this.m_Doc = groups.First().Document;
            }
            this.TextBoxViewPlan.Text = view.Name;
            this.m_UnSelectedGroups = groups;
            this.m_UnSelectedGroups.Sort(new GroupComparer());
            this.m_SelectedGroups = new List<Group>();
            // this.UpdateListView(this.ListViewLeft, this.m_UnSelectedGroups);
            // this.UpdateListView(this.ListViewRight, this.m_UnSelectedGroups);
        }

        // private void UpdateListView(ListView listView, List<Group> groups)
        // {
        //     listView.ItemsSource = groups;
        //     
        //
        // }
    }
}