using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using LearnWpfMVVM;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB.Structure;

namespace CurvedBeamWpf.ViewModel
{
    public class CurvedBeamViewModel : NotificationObject
    {
        private Document doc = null;
        private Action _closeAction = null;

        #region ����

        //level

        private List<Level> _levelTypes;

        public List<Level> LevelTypes
        {
            get => _levelTypes;
            set
            {
                _levelTypes = value;
                RaisePropertyChanged("LevelTypes");
            }
        }

        private List<string> _levelTypesName;

        public List<string> LevelTypesName
        {
            get => _levelTypesName;
            set
            {
                _levelTypesName = value;
                RaisePropertyChanged("LevelTypesName");
            }
        }

        public Level LevelType = null;

        private string _currentSelectOflevel;

        public string CurrentSelectOfLevel
        {
            get { return _currentSelectOflevel; }
            set
            {
                _currentSelectOflevel = value;
                LevelType = _levelTypes.FirstOrDefault(x => x.Name == _currentSelectOflevel) as Level;
                RaisePropertyChanged("CurrentSelectOfLevel");
            }
        }

        private List<FamilySymbol> _BeamTypes;

        public List<FamilySymbol> BeamTypes
        {
            get => _BeamTypes;
            set
            {
                _BeamTypes = value;
                RaisePropertyChanged("BeamTypes");
            }
        }

        private List<string> _beamTypesName;

        public List<string> BeamTypesName
        {
            get => _beamTypesName;
            set
            {
                _beamTypesName = value;
                RaisePropertyChanged("FloorTypesName");
            }
        }

        // �������õ�BeamType
        public FamilySymbol BeamType = null;

        private string _currentSelectOfBeam;

        public string CurrentSelectOfBeam
        {
            get { return _currentSelectOfBeam; }
            set
            {
                _currentSelectOfBeam = value;
                BeamType = BeamTypes.FirstOrDefault(x => x.Name == _currentSelectOfBeam);
                RaisePropertyChanged("CurrentSelectOfBeam");
            }
        }

        #endregion


        public CurvedBeamViewModel(ExternalCommandData commandData, Action closeAction)
        {
            doc = commandData.Application.ActiveUIDocument.Document;
            _closeAction = closeAction;

            InitializeData(); //��ʼ����������

            StartCreateArcCommand = new DelegateCommand();
            StartCreateArcCommand.ExecuteAction = new Action<object>(CreateBeamArc);

            StartCreateBeamEllispeCommand = new DelegateCommand();
            StartCreateBeamEllispeCommand.ExecuteAction = new Action<object>(CreateBeamEllispe);

            // Transaction ts = new Transaction(doc, "mvvm��ִ������Ĳ���");
            // ts.Start();
            // BeamType = BeamTypes.FirstOrDefault(x => x.Name == _currentSelectOfBeam);
            // string info = BeamType.Name == null ? "ѡ���������û�а���" : BeamType.Name;
            // TaskDialog.Show("tips", info);
            // ts.Commit();
        }


        public DelegateCommand StartCreateArcCommand { get; set; }
        public DelegateCommand StartCreateBeamEllispeCommand { get; set; }

        public string info = null;

        void InitializeData()
        {
            //Beam:
            BeamTypes = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_StructuralFraming)
                .OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>()
                .ToList();

            BeamTypesName = BeamTypes.ConvertAll(x => x.Name);
            CurrentSelectOfBeam = BeamTypesName.First();

            // Level:
            LevelTypes = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Levels)
                .OfClass(typeof(Level))
                .Cast<Level>()
                .ToList();
            LevelTypesName = LevelTypes.ConvertAll(x => x.Name);
            CurrentSelectOfLevel = LevelTypesName.First();
        }


        void CreateBeamArc(object parameter)
        {
            var curve = Line.CreateBound(new XYZ(0, 0, 0), new XYZ(100, 0, 0));

            Transaction ts = new Transaction(doc, "mvvm��ִ������Ĳ���");
            ts.Start();

            BeamType = BeamTypes.FirstOrDefault(x => x.Name == _currentSelectOfBeam);

            if (!BeamType.IsActive)
            {
                BeamType.Activate();
            }

            LevelType = _levelTypes.FirstOrDefault(x => x.Name == _currentSelectOflevel) as Level;
            // if (!LevelType.IsActive)
            // {
            //     symbol.Activate();
            // }

            doc.Create.NewFamilyInstance(curve, BeamType, LevelType, StructuralType.Beam);
            TaskDialog.Show("tips", "����������");

            ts.Commit();

        }


        void CreateBeamEllispe(object parameter)
        {
            string info = CurrentSelectOfBeam + "\n";
            info += BeamType.Name + "\n";

            // LevelType = _levelTypes.FirstOrDefault(x => x.Name == _currentSelectOflevel) as Level;

            info += CurrentSelectOfLevel + "\n";
            info += LevelType.Name + "\n";
            
            TaskDialog.Show("tips", info);
        }


        void ExcuteCreateBeamSplineCommand()
        {
            //*****************8
        }
    }
}