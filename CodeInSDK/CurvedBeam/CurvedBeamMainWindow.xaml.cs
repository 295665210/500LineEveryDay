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


namespace CurvedBeam.View
{
    /// <summary>
    /// CurvedBeamMainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CurvedBeamMainWindow : Window
    {
      
        public CurvedBeamMainWindow(ExternalCommandData commandData)
        {
            InitializeComponent();
            // this.DataContext = new CurvedBeamViewModel(commandData, Close);
        }

       

        private void ArcBtn_OnClick(object sender, RoutedEventArgs e)
        {
            // Level locLev =LevleCB.SelectedItem.BeamMaps;
        }

        private void EllipseBtn_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SplineBtn_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}