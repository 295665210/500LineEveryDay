// using System;
// using System.CodeDom.Compiler;
// using System.ComponentModel;
// using System.Diagnostics;
// using System.Windows;
// using System.Windows.Controls;
// using System.Windows.Input;
// using System.Windows.Markup;
//
// namespace RevitFoundation.CodeInQuick.Source.UpPipe
// {
// 	// Token: 0x02000015 RID: 21
// 	public partial class UpPipeWpf : Window, IComponentConnector
// 	{
// 		// Token: 0x0600005F RID: 95 RVA: 0x00002412 File Offset: 0x00000612
// 		public UpPipeWpf()
// 		{
// 			this.InitializeComponent();
// 		}
//
// 		// Token: 0x06000060 RID: 96 RVA: 0x00002420 File Offset: 0x00000620
// 		private void Button_Click(object sender, RoutedEventArgs e)
// 		{
// 			if (this.TextboxHight != null)
// 			{
// 				string text = this.TextboxHight.Text;
// 				base.DialogResult = new bool?(true);
// 				base.Close();
// 			}
// 		}
//
// 		// Token: 0x06000061 RID: 97 RVA: 0x0000244B File Offset: 0x0000064B
// 		private void Window_Loaded(object sender, RoutedEventArgs e)
// 		{
// 			this.TextboxHight.Focus();
// 			this.TextboxHight.SelectionStart = 0;
// 			this.TextboxHight.SelectionLength = this.TextboxHight.Text.Length;
// 		}
//
// 		// Token: 0x06000062 RID: 98 RVA: 0x00002480 File Offset: 0x00000680
// 		private void TextboxHight_PreviewKeyDown(object sender, KeyEventArgs e)
// 		{
// 			if (e.Key == Key.Return || e.Key == Key.Space)
// 			{
// 				this.Button_Click(sender, e);
// 			}
// 		}
//
// 		// Token: 0x06000063 RID: 99 RVA: 0x00006CA0 File Offset: 0x00004EA0
// 		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
// 		[DebuggerNonUserCode]
// 		public void InitializeComponent()
// 		{
// 			if (!this._contentLoaded)
// 			{
// 				this._contentLoaded = true;
// 				Uri resourceLocator = new Uri("/快速弹夹2018;component/03%e6%9c%ba%e7%94%b5/uppipewpf.xaml", UriKind.Relative);
// 				Application.LoadComponent(this, resourceLocator);
// 			}
// 		}
//
// 		// Token: 0x06000064 RID: 100 RVA: 0x00006CD0 File Offset: 0x00004ED0
// 		[EditorBrowsable(EditorBrowsableState.Never)]
// 		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
// 		[DebuggerNonUserCode]
// 		void IComponentConnector.Connect(int connectionId, object target)
// 		{
// 			switch (connectionId)
// 			{
// 			case 1:
// 				((UpPipeWpf)target).Loaded += this.Window_Loaded;
// 				break;
// 			case 2:
// 				this.TextboxHight = (TextBox)target;
// 				this.TextboxHight.PreviewKeyDown += this.TextboxHight_PreviewKeyDown;
// 				break;
// 			case 3:
// 				this.enter3 = (Button)target;
// 				this.enter3.Click += this.Button_Click;
// 				break;
// 			default:
// 				this._contentLoaded = true;
// 				break;
// 			}
// 		}
//
// 		// Token: 0x04000042 RID: 66
// 		internal TextBox TextboxHight;
//
// 		// Token: 0x04000043 RID: 67
// 		internal Button enter3;
//
// 		// Token: 0x04000044 RID: 68
// 		private bool _contentLoaded;
// 	}
// }
