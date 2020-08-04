using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;

namespace ExtensibleStorageManager.User
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class UICommand : Window
    {
#region Constructor
        public UICommand(Document doc, string applicationId)
        {
            InitializeComponent();
            this.Closing += new System.ComponentModel.CancelEventHandler(UICommand_closing);

        }
        #endregion


#region Properties
        /// <summary>
        /// The active document in Revit that the dialog queries for Schema and Entity data.
        /// </summary>
        public Document Document
        {
            get{}
        }

        #endregion

#region  Data
        private  
#endregion
    }
}