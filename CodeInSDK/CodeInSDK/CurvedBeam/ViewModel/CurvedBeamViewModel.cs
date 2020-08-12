using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CurvedBeam.ViewModel
{
    public class CurvedBeamViewModel
    {
#region Class memeber variables
        private UIApplication m_revit = null;
        List<FamilySymbol> m_beamMaps = new List<FamilySymbol>(); //list of beams's type  
        List<Level> m_levels = new List<Level>();                 //list of levels
#endregion

#region Command class properties
        //list of all type of beams
        public List<FamilySymbol> BeamMaps
        {
            get
            {
                return m_beamMaps;
            }
        }
        public List<Level> LevelMaps
        {
            get { return m_levels; }
        }
#endregion


        public CurvedBeamViewModel(ExternalCommandData commandData)
        {
            m_revit = commandData.Application;
            Initialize();
        }

        private void Initialize()
        {
            Document doc = m_revit.ActiveUIDocument.Document;
            m_beamMaps = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_StructuralFraming)
                .OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>()
                .ToList();

            m_levels = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Levels)
                .OfClass(typeof(Level))
                .Cast<Level>()
                .ToList();
        }

        public void ViewModelRun(string message)
        {
            Transaction ts = new Transaction(m_revit.ActiveUIDocument.Document);
            ts.Start();
            MessageBox.Show(message);
            ts.Commit();
        }
    }
}