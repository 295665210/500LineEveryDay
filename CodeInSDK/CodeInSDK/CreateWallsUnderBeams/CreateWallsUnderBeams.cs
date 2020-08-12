using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using View = Autodesk.Revit.DB.View;

namespace CreateWallsUnderBeams
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class CreateWallsUnderBeams : IExternalCommand
    {
        //private Members 字段
        private IList<WallType> m_wallTypeCollection; //store all the wall types in current document
        private ArrayList m_beamCollection;           //store the selection of beams in Revit;
        private WallType m_selectedWallType;          //store the selected wall type;
        private Level m_level;                        //store the level which wall create on
        private Boolean m_isStructural;               //indicate whether create structural wall
        private string m_errorInformation;
        private const Double PRECISION = 0.000000001;

        //Properties
        /// <summary>
        /// inform the wall type selected by the user
        /// </summary>
        public IList<WallType> WallTypes
        {
            get { return m_wallTypeCollection; }
        }
        /// <summary>
        /// inform the wall type selected by the user
        /// </summary>
        public object SelectedWallType
        {
            set
            {
                m_selectedWallType = value as WallType;
            }
        }

        public Boolean IsStructural
        {
            get { return m_isStructural; }
            set { m_isStructural = value; }
        }

        //Methods
        /// <summary>
        /// default constructor of createWallsUnderBeams
        /// </summary>
        public CreateWallsUnderBeams()
        {
            m_wallTypeCollection = new List<WallType>();
            m_beamCollection = new ArrayList();
            m_isStructural = true;
        }


        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication revit = commandData.Application;
            UIDocument project = revit.ActiveUIDocument;

            //find the selection of beams in revit
            ElementSet selection = new ElementSet();
            foreach (ElementId elementId in project.Selection.GetElementIds())
            {
                selection.Insert(project.Document.GetElement(elementId));
            }
            foreach (Element e in selection)
            {
                FamilyInstance m = e as FamilyInstance;
                if (null != m)
                {
                    if (StructuralType.Beam == m.StructuralType)
                    {
                        //store all the beams the user selected in revit
                        m_beamCollection.Add(e);
                    }
                }
            }

            if (0 == m_beamCollection.Count)
            {
                message = "Can not find any beams.";
                return Result.Failed;
            }

            //make sure all the beams have horizontal analytical line
            //?????????????为什么用 analytical line
            if (!CheckBeamHorizontal())
            {
                message = "Can not find any beams";
                return Result.Failed;
            }
            //search all the wall types in the Revit.
            FilteredElementCollector filteredElementCollector = new FilteredElementCollector(project.Document);
            filteredElementCollector.OfClass(typeof(WallType));
            m_wallTypeCollection = filteredElementCollector.Cast<WallType>().ToList<WallType>();

#region 这段代码要用到winform窗体，先注释掉。
            


            //show the dialog for the user select the wall type
            // using (CreateWallsUnderBeamsForm displayForm = new CreateWallsUnderBeamsForm(this))
            // {
            //     if (DialogResult.OK != displayForm.ShowDialog())
            //     {
            //         return Result.Failed;
            //     }
            // }

#endregion



            //Create the walls which along and under the path of the beams.
            if (!BeginCreate(project.Document))
            {
                message = m_errorInformation;
                return Result.Failed;
            }
            return Result.Succeeded;
        }

        private bool BeginCreate(Document project)
        {
            //begin to create walls along and under each beam
            for (int i = 0; i < m_beamCollection.Count; i++)
            {
                //get each selected beam.
                FamilyInstance m = m_beamCollection[i] as FamilyInstance;
                if (null == m)
                {
                    m_errorInformation = "The program should not go here.";
                    return false;
                }

                //Get the analytical model of the beam.
                //the wall will be created using thsi model line as path
                AnalyticalModel model = m.GetAnalyticalModel();
                if (null == model)
                {
                    m_errorInformation = "The beam should have analytical model.";
                    return false;
                }
                //get the level using the beam's reference level
                ElementId levelId = m.get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM).AsElementId();
                m_level = project.GetElement(levelId) as Level;
                if (null == m_level)
                {
                    m_errorInformation = "The program should not go here.";
                    return false;
                }

                Transaction ts = new Transaction(project, Guid.NewGuid().GetHashCode().ToString());
                ts.Start();
                Wall createdWall = Wall.Create(project, model.GetCurve(), m_selectedWallType.Id, m_level.Id, 10, 0,
                    true,
                    m_isStructural);
                if (null == createdWall)
                {
                    m_errorInformation = "Can not create the walls";
                    return false;
                }

                //Modify some parameters of the created wall to make it look better.
                Double offset = model.GetCurve().GetEndPoint(0).Z - m_level.Elevation;
                createdWall.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT).Set(levelId);
                createdWall.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET).Set(offset - 300 / 304.8);
                createdWall.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).Set(levelId);
                ts.Commit();
            }
            return true;
        }


        private bool CheckBeamHorizontal()
        {
            for (int i = 0; i < m_beamCollection.Count; i++)
            {
                //get the analytical curve of each selected beam.
                //and check if Z coordinate of start point and end point of the curve are same.
                FamilyInstance m = m_beamCollection[i] as FamilyInstance;
                AnalyticalModel model = m.GetAnalyticalModel();
                if (null == model)
                {
                    m_errorInformation = "The beam should have analytical model.";
                    return false;
                }
                else if ((PRECISION <= model.GetCurve().GetEndPoint(0).Z - model.GetCurve().GetEndPoint(1).Z)
                    || (-PRECISION >= model.GetCurve().GetEndPoint(0).Z - model.GetCurve().GetEndPoint(1).Z))
                {
                    m_errorInformation = "Please only selcect horizontal beams.";
                    return false;
                }
            }
            return true;
        }
    }
}