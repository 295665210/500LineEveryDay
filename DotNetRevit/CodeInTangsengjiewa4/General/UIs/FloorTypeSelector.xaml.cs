using System.ComponentModel;
using System.Windows;

namespace CodeInTangsengjiewa4.General.UIs
{
    /// <summary>
    /// UserControl2.xaml 的交互逻辑
    /// </summary>
    public partial class FloorTypeSelector : Window
    {
        public FloorTypeSelector()
        {
            InitializeComponent();
        }

        private static FloorTypeSelector instance;

        public static FloorTypeSelector Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FloorTypeSelector();
                }

                return instance;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            //base.OnClosing(e);
        }
    }
}