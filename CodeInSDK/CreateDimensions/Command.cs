using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using View = Autodesk.Revit.DB.View;

namespace CreateDimensions
{
    /// <summary>
    /// 运行不起来， sdk里的代码也运行不起来 尴尬呀
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class Command : IExternalCommand
    {
        private ExternalCommandData m_revit = null;
        string m_errorMessage = "";
        ArrayList m_walls = new ArrayList();
        private const double precision = 0.000001;


        public Result Execute(ExternalCommandData revit, ref string message, ElementSet elements)
        {
            try
            {
                m_revit = revit;
                View view = m_revit.Application.ActiveUIDocument.Document.ActiveView;
                View3D view3D = view as View3D;
                if (null != view3D)
                {
                    message += "Only create dimension in 2D";
                    return Result.Failed;
                }

                ViewSheet viewSheet = view as ViewSheet;
                if (null != viewSheet)
                {
                    message += "Only create dimension in 2D";
                    return Result.Failed;
                }

                //try to adds a dimension from the start of the wall to the end of the wall into the project

                if (!AddDimension())
                {
                    message = m_errorMessage;
                    return Result.Failed;
                }
                return Result.Succeeded;
            }
            catch (Exception e)
            {
                // message = e.Message;
                MessageBox.Show(e.ToString());
                return Result.Failed;
            }
        }


        bool initialize()
        {
            ElementSet selections = new ElementSet();
            foreach (var elementId in m_revit.Application.ActiveUIDocument.Selection.GetElementIds())
            {
                selections.Insert(m_revit.Application.ActiveUIDocument.Document.GetElement(elementId));
            }
            //if nothing was selected
            if (0 == selections.Size)
            {
                m_errorMessage += "please seelect Basic walls";
                return false;
            }

            //find out wall
            foreach (Element element in selections)
            {
                Wall wall = element as Wall;
                if (null != wall)
                {
                    if ("Basic" != wall.WallType.Kind.ToString())
                    {
                        continue;
                    }
                    m_walls.Add(wall);
                }
            }

            //no walls wal selected
            if (0 == m_walls.Count)
            {
                m_errorMessage += "Please select Basic Walls";
                return false;
            }
            return true;
        }

        public bool AddDimension()
        {
            initialize();
            if (!initialize())
            {
                return false;
            }

            Transaction transaction = new Transaction(m_revit.Application.ActiveUIDocument.Document, "Add dimensions");
            transaction.Start();
            //get out all the walls in this array, and create a dimension from its start to its end
            for (int i = 0; i < m_walls.Count; i++)
            {
                Wall wallTemp = m_walls[i] as Wall;
                if (null == wallTemp)
                {
                    continue;
                }

                //get location curve
                Location location = wallTemp.Location;
                LocationCurve locationLine = location as LocationCurve;
                if (null == locationLine)
                {
                    continue;
                }

                //new line
                Line newLine = null;

                //get reference
                ReferenceArray referenceArray = new ReferenceArray();
                AnalyticalModel analyticalModel = wallTemp.GetAnalyticalModel();
                IList<Curve> activeCurveList = analyticalModel.GetCurves(AnalyticalCurveType.ActiveCurves);

                foreach (Curve aCurve in activeCurveList)
                {
                    //find non-vertical curve from analytical model
                    if (aCurve.GetEndPoint(0).Z == aCurve.GetEndPoint(1).Z)
                    {
                        newLine = aCurve as Line;
                    }

                    //？？？？？？？？？？？这是在干啥？？？？？？？？？？？
                    if (aCurve.GetEndPoint(0).Z != aCurve.GetEndPoint(1).Z)
                    {
                        AnalyticalModelSelector amSelector = new AnalyticalModelSelector(aCurve);
                        amSelector.CurveSelector = AnalyticalCurveSelector.StartPoint;
                        referenceArray.Append(analyticalModel.GetReference(amSelector));
                    }
                    if (2 == referenceArray.Size)
                    {
                        break;
                    }
                }

                if (referenceArray.Size != 2)
                {
                    m_errorMessage += "Did not find two reference";
                    return false;
                }

                try
                {
                    //try to add a new dimension
                    UIApplication app = m_revit.Application;
                    Document doc = app.ActiveUIDocument.Document;

                    XYZ p1 = new XYZ(newLine.GetEndPoint(0).X + 5,
                        newLine.GetEndPoint(0).Y + 5,
                        newLine.GetEndPoint(0).Z);
                    XYZ p2 = new XYZ(newLine.GetEndPoint(1).X + 5,
                        newLine.GetEndPoint(1).Y + 5,
                        newLine.GetEndPoint(1).Z);
                    Line newLine2 = Line.CreateBound(p1, p2);
                    Dimension newDimension = doc.Create.NewDimension(doc.ActiveView, newLine2, referenceArray);
                }
                catch (Exception ex)
                {
                    // m_errorMessage += ex.ToString();
                    MessageBox.Show(ex.ToString());
                    return false;
                }
            }
            transaction.Commit();
            return true;
        }
    }
}