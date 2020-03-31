using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Grid = Autodesk.Revit.DB.Grid;
using MessageBox = System.Windows.MessageBox;


namespace CodeInTangsengjiewa4.CodeInFunny.GridCreation
{
    /// <summary>
    /// Interaction logic for FormCreation.xaml
    /// </summary>
    public partial class FormCreation : Window
    {
        private UIApplication _revitApp;

        public FormCreation(UIApplication revitApp)
        {
            InitializeComponent();
            _revitApp = revitApp;
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            //check input first;
            try
            {
                List<double> horiSpaces = ParseSpaces(HoriSpaces.Text);
                List<double> vertSpaces = ParseSpaces(VertSpaces.Text);
                string horiName = HoriStart.Text;
                if (string.IsNullOrEmpty(horiName))
                {
                    horiName = "A";

                }
                string vertName = VertStart.Text;
                if (string.IsNullOrEmpty(vertName))
                {
                    vertName = "1";
                }

                Visibility = System.Windows.Visibility.Hidden;
                XYZ basePoint = SelectBasePoint();
                if (basePoint == null)
                {
                    Visibility = System.Windows.Visibility.Visible;
                    return;
                }

                CreateGrids(basePoint, horiSpaces, horiName, true);
                CreateGrids(basePoint, vertSpaces, vertName,false);
                Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error", ex.Message);
                Visibility = System.Windows.Visibility.Visible;
            }
        }

        /// <summary>
        /// create parallel grids(horizontal or vertical)
        /// </summary>
        /// <param name="basePoint"></param>
        /// <param name="spaces"></param>
        /// <param name="startName"></param>
        /// <param name="isHorizontal"></param>
        private void CreateGrids(XYZ basePoint, List<double> spaces, string startName, bool isHorizontal)
        {
            Document doc = _revitApp.ActiveUIDocument.Document;
            double gridLength = 30, extLength = 3; //unit : feet
            using (Transaction t = new Transaction(doc, "create grids"))
            {
                t.Start();
                XYZ offsetDir = isHorizontal ? XYZ.BasisY.Multiply(-1) : XYZ.BasisX;
                XYZ startPoint = isHorizontal
                    ? basePoint.Add(XYZ.BasisX.Multiply(-extLength))
                    : basePoint.Add(XYZ.BasisY.Multiply(extLength));
                XYZ endPoint = isHorizontal
                    ? startPoint.Add(XYZ.BasisX.Multiply(gridLength))
                    : startPoint.Add(XYZ.BasisY.Multiply(-gridLength));

                Line geoLine = Line.CreateBound(startPoint, endPoint);
                Grid grid = Grid.Create(doc, geoLine);
                grid.Name = startName;

                foreach (double space in spaces)
                {
                    startPoint = startPoint.Add(offsetDir.Multiply(space));
                    endPoint = endPoint.Add(offsetDir.Multiply(space));
                    geoLine = Line.CreateBound(startPoint, endPoint);
                    Grid.Create(doc, geoLine);
                }
                t.Commit();
            }
        }

        /// <summary>
        /// 选择放置的位置
        /// </summary>
        /// <returns></returns>
        private XYZ SelectBasePoint()
        {
            try
            {
                return _revitApp.ActiveUIDocument.Selection.PickPoint("please select a base point");
            }
            catch (Exception)
            {
                return null;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private List<double> ParseSpaces(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new Exception("Invalid spaces input!");
            }

            string[] sps = value.Split(' ');
            List<double> spaces = new List<double>();
            foreach (string s in sps)
            {
                double val = Convert.ToDouble(s);
                if (val <= 0)
                {
                    throw new Exception("Non positive space!");
                }
                spaces.Add(val);
            }
            return spaces;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}