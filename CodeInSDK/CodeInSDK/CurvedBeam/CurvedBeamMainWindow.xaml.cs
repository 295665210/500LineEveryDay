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
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CurvedBeam.ViewModel;


namespace CurvedBeam.View
{
    /// <summary>
    /// CurvedBeamMainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CurvedBeamMainWindow : Window
    {
        private ExternalCommandData m_revit = null;
        private CurvedBeamViewModel viewmodel = null;
        private List<FamilySymbol> m_beamMaps;
        private List<Level> m_levels;

        private CurvedBeamMainWindow()
        {
            InitializeComponent();
        }

        public CurvedBeamMainWindow(ExternalCommandData commandData)
        {
            m_revit = commandData;
            InitializeComponent();
            viewmodel = new CurvedBeamViewModel(commandData);
            m_beamMaps = viewmodel.BeamMaps;
            m_levels = viewmodel.LevelMaps;

            this.DataContext = viewmodel;
        }


        private void ArcBtn_OnClick(object sender, RoutedEventArgs e)
        {
            // Level level = LevleCB.SelectedItem as Level;
            // string levelName = level.Name;
            // MessageBox.Show(levelName);
            //如果执行个一个revit的命令
            Document doc = m_revit.Application.ActiveUIDocument.Document;
            Transaction ts2 = new Transaction(m_revit.Application.ActiveUIDocument.Document,"在wpf里执行revit的命令");
            ts2.Start();
            var curve = Autodesk.Revit.DB.Line.CreateBound(new XYZ(0, 0, 0), new XYZ(100, 0, 0));

            var beamType = BeamTypeCB.SelectedItem as FamilySymbol;
            if (!beamType.IsActive)
            {
                beamType.Activate();
            }

            var levelType = LevleCB.SelectedItem as Level;

            doc.Create.NewFamilyInstance(curve, beamType, levelType, StructuralType.Beam);
            TaskDialog.Show("tips", "梁创建好了");
            ts2.Commit();
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