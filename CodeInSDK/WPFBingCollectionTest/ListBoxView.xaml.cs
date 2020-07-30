using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace WPFBingCollectionTest
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class ListBoxView : Window
    {
        ObservableCollection<Element> collection = new ObservableCollection<Element>();
        public ObservableCollection<Element> Collection
        {
            get { return collection; }
            set { collection = value; }
        }

        public ListBoxView(ExternalCommandData commandData)
        {
            InitializeComponent();

            List<Element> pickedElements = new List<Element>();
            foreach (ElementId elementId in commandData.Application.ActiveUIDocument.Selection.GetElementIds())
            {
                
                pickedElements.Add(commandData.Application.ActiveUIDocument.Document.GetElement(elementId));
            }

            foreach (Element element in pickedElements)
            {
               
                collection.Add(element);
            }

            this.DataContext = this;
        }
    }
}