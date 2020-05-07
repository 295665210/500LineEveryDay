using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace RevitFoundation.CodeInQuick
{
    /// <summary>
    /// CreatehengjiaRebar.xaml 的交互逻辑
    /// </summary>
    public partial class CreatehengjiaRebarWPF : Window, IComponentConnector
    {
        public CreatehengjiaRebarWPF()
        {
            InitializeComponent();
            this.A80.IsChecked = new bool?(true);
            this.vertical.IsChecked = new bool?(true);
        }

        private void type_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton) sender;
            this.m_type.Text = radioButton.Name.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.totalLength.Text.CheckString2() && this.subLength.Text.CheckString2())
            {
                this.Iscontinue = true;
                base.Close();
            }
            else
            {
                MessageBox.Show("请检查输入值!");
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                this.Iscontinue = true;
                base.Close();
            }
            else if (e.Key == Key.Escape)
            {
                base.Close();
            }
        }

        // //这是干啥用的
        // [EditorBrowsable(EditorBrowsableState.Never)]
        // [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        // [DebuggerNonUserCode]
        void IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
            case 1:
                ((CreatehengjiaRebarWPF) target).PreviewKeyDown += this.Window_PreviewKeyDown;
                break;
            case 2:
                this.A80 = (RadioButton) target;
                this.A80.Checked += this.type_Checked;
                break;
            case 3:
                this.A90 = (RadioButton) target;
                this.A90.Checked += this.type_Checked;
                break;
            case 4:
                this.A100 = (RadioButton) target;
                this.A100.Checked += this.type_Checked;
                break;
            case 5:
                this.B80 = (RadioButton) target;
                this.B80.Checked += this.type_Checked;
                break;
            case 6:
                this.B90 = (RadioButton) target;
                this.B90.Checked += this.type_Checked;
                break;
            case 7:
                this.B100 = (RadioButton) target;
                this.B100.Checked += this.type_Checked;
                break;
            case 8:
                this.m_type = (TextBlock) target;
                break;
            case 9:
                this.totalLength = (TextBox) target;
                break;
            case 10:
                this.subLength = (TextBox) target;
                break;
            case 11:
                this.vertical = (RadioButton) target;
                break;
            case 12:
                this.horizon = (RadioButton) target;
                break;
            case 13:
                ((Button) target).Click += this.Button_Click;
                break;
            default:
                this._contentLoaded = true;
                break;
            }
        }


        public bool Iscontinue = false;


        internal RadioButton A80;


        internal RadioButton A90;

        internal RadioButton A100;


        internal RadioButton B80;


        internal RadioButton B90;


        internal RadioButton B100;


        internal TextBlock m_type;


        internal TextBox totalLength;


        internal TextBox subLength;


        internal RadioButton vertical;

        internal RadioButton horizon;

        private bool _contentLoaded;

   
    }
}