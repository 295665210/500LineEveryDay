using System.ComponentModel;
using System.Windows;

namespace CodeInTangsengjiewa3.UIs
{
    /// <summary>
    /// </summary>
    public partial class AboutForm : Window
    {
        private static AboutForm instance;

        public static AboutForm Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AboutForm();
                }

                return instance;
            }
        }

        public AboutForm()
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