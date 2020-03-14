using System.ComponentModel;
using System.Windows;

namespace RevitFoundation.PracticeBookInRevit.UIs
{
    /// <summary>
    /// ViewSelector.xaml 的交互逻辑
    /// </summary>
    public partial class ViewSelector : Window
    {
        private static ViewSelector instance;

        public static ViewSelector Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ViewSelector();
                }

                return instance;
            }
        }

        public ViewSelector()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            //base.OnClosing(e);
        }
    }
}