using System.ComponentModel;
using System.Windows;

namespace 唐僧解瓦.机电.ToolUIs
{
    /// <summary>
    /// About.xaml 的交互逻辑
    /// </summary>
    public partial class ValueSettingUI : Window
    {
        public ValueSettingUI()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            base.OnClosing(e);
        }

        private void OkBtOkbtn_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}