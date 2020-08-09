using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

namespace ElementsBatchCreation
{
    using View = Autodesk.Revit.DB.View;

    /// <summary>
    /// This class will demonstrate how to create many elements via batch creation methods.
    /// </summary>
    public class ElementsBatchCreation
    {
        /// <summary>
        /// A reference to the external application
        /// </summary>
        public ExternalCommandData m_cmdData;
        /// <summary>
        /// A reference to active document.
        /// </summary>
        public Autodesk.Revit.DB.Document m_doc;
        /// <summary>
        /// A reference to Level 1
        /// </summary>
        public Level m_level;
        /// <summary>
        /// A reference to ViewPlan named "Level 1";
        /// </summary>
        private ViewPlan m_viewPlan;

#region constructor
        /// <summary>
        /// Constructor of ElementBatchCreation
        /// </summary>
        /// <param name="cmdData"></param>
        public ElementsBatchCreation(ExternalCommandData cmdData)
        {
            m_cmdData = cmdData;
            m_doc = cmdData.Application.ActiveUIDocument.Document;
        }
#endregion

        /// <summary>
        /// Batch creations of several elements, it will call separate methods for each element
        /// </summary>
        /// <returns>if all batch creations succeed,return true;otherwise, return false</returns>
        public bool CreatElements()
        {
            //Get common information for batch creation
            Transaction tran = new Transaction(m_doc, "Elements Batch Creation");
            tran.Start();
            PreCreate();
            m_doc.AutoJoinElements();
            m_doc.Regenerate();
            tran.Commit();

            //prepare messages to notify user of succeed and failed options
            String failedMethods = "";
            String succeedMethods = "";
            bool success = false;
            tran.Start();
            success = CreateAreas();
            m_doc.AutoJoinElements();
            m_doc.Regenerate();
            tran.Commit();
            if (success)
            {
                succeedMethods += " Area";
            }
            else
            {
                failedMethods += " Area";
            }

            //
            tran.Start();
            success = CreateColumns();
            m_doc.AutoJoinElements();
            m_doc.Regenerate();
            tran.Commit();
            if (success)
            {
                succeedMethods += " Column";
            }
            else
            {
                failedMethods += " Column";
            }

            //
            tran.Start();
            success = CreateRooms();
            m_doc.AutoJoinElements();
            m_doc.Regenerate();
            tran.Commit();
            if (success)
            {
                succeedMethods += " Room";
            }
            else
            {
                failedMethods += " Room";
            }

            //
            tran.Start();
            success = CreateTextNotes();
            tran.Commit();
            if (success)
            {
                succeedMethods += " TextNote";
            }
            else
            {
                failedMethods += " TextNote";
            }

            //
            tran.Start();
            success = CreateWalls();
            m_doc.AutoJoinElements();
            m_doc.Regenerate();
            tran.Commit();
            if (success)
            {
                succeedMethods += " Wall";
            }
            else
            {
                failedMethods += " Wall";
            }

            bool result = true;
            if (String.IsNullOrEmpty(succeedMethods))
            {
                TaskDialog.Show("ElementsBatchCreation", "Batch creations of" + failedMethods + "failed",
                    TaskDialogCommonButtons.Close);
            }
            else if (String.IsNullOrEmpty(failedMethods))
            {
                TaskDialog.Show("ElementsBatchCreation", "Batch creations of" + succeedMethods + " succeed",
                    TaskDialogCommonButtons.Close);
            }
            else
            {
                TaskDialog.Show("ElementsBatchCreation",
                    "Batch creations of" + succeedMethods + " succeed," + " Batch creations of" + failedMethods +
                    " failed", TaskDialogCommonButtons.Close);
            }
            return result;
        }

        /// <summary>
        /// Get common information for batch creation
        /// </summary>
        private void PreCreate()
        {
            try
            {
                Autodesk.Revit.Creation.Application appCreation = m_cmdData.Application.Application.Create;
                //Try to get level named "Level 1" which will be used in most creations
                m_level =
                (
                from elem in
                    new FilteredElementCollector(m_doc).OfClass(typeof(Level)).ToElements()
                let level = elem as Level
                where level != null && "Levle 1" == level.Name
                select level
                ).First();
                //If ViewPlan "Level 1" does not exist ,try to create one.
                if (null != m_level)
                {
                    ElementId AreaSchemeId = new FilteredElementCollector(m_doc).OfClass(typeof(AreaScheme))
                        .FirstOrDefault(a => a.Name == "Rentable").Id;
                    try
                    {
                        m_viewPlan = ViewPlan.CreateAreaPlan(m_doc, AreaSchemeId, m_level.Id);
                        m_viewPlan.Name = "Level 1";
                    }
                    catch
                    {
                    }
                }

                if (null == m_level && null == m_viewPlan)
                {
                    return;
                }

                //List of curve is used to store Area's boundary lines
                List<Curve> curves = new List<Curve>();
                XYZ pt1 = new XYZ(-4, 95, 0);
                XYZ pt2 = new XYZ(-106, 95, 0);
                Line line = Line.CreateBound(pt1, pt2);
                curves.Add(line);
                pt1 = new Autodesk.Revit.DB.XYZ(-4, 105, 0);
                pt2 = new Autodesk.Revit.DB.XYZ(-106, 105, 0);
                line = Line.CreateBound(pt1, pt2);
                curves.Add(line);

                for (int i = 0; i < 11; i++)
                {
                    pt1 = new XYZ(-5 - i * 10, 94, 0);
                    pt2 = new XYZ(-5 - i * 10, 106, 0);
                    line = Line.CreateBound(pt1, pt2);
                    if (null != line)
                    {
                        curves.Add(line);
                    }
                }

                /* Create Area Boundary Line for Area.
                It is necessary if need to create closed region for Area.
                But for room ,it is not necessary.
                 */
                XYZ origin = new XYZ(0, 0, 0);
                XYZ norm = new XYZ(0, 0, 1);
                Plane plane = Plane.CreateByNormalAndOrigin(norm, origin);
                if (null != plane)
                {
                    SketchPlane sketchPlane = SketchPlane.Create(m_doc, plane);
                    if (null != sketchPlane)
                    {
                        foreach (Curve curve in curves)
                        {
                            m_doc.Create.NewAreaBoundaryLine(sketchPlane, curve, m_viewPlan);
                        }
                    }
                }

                //Create enclosed region using walls for room
                pt1 = new Autodesk.Revit.DB.XYZ(5, -5, 0);
                pt2 = new Autodesk.Revit.DB.XYZ(55, -5, 0);
                line = Line.CreateBound(pt1, pt2);
                Wall.Create(m_doc, line, m_level.Id, true);

                pt1 = new Autodesk.Revit.DB.XYZ(5, 5, 0);
                pt2 = new Autodesk.Revit.DB.XYZ(55, 5, 0);
                line = Line.CreateBound(pt1, pt2);
                Wall.Create(m_doc, line, m_level.Id, true);

                for (int i = 0; i < 6; i++)
                {
                    pt1 = new XYZ(5 + i * 10, -5, 0);
                    pt2 = new XYZ(5 + i * 10, 5, 0);
                    line = Line.CreateBound(pt1, pt2);
                    Wall.Create(m_doc, line, m_level.Id, true);
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Batch creation of areas.
        /// </summary>
        /// <returns></returns>
        private bool CreateAreas()
        {
            try
            {
                if (null == m_viewPlan)
                {
                    return false;
                }
                List<AreaCreationData> areaCreationDatas = new List<AreaCreationData>();
                //create AreaCreateDatas for Area' Batch creation
                for (int i = 1; i < 11; i++)
                {
                    UV point = new UV(i * -10, 100);
                    UV tagPoint = new UV(i * -10, 100);
                    AreaCreationData areaCreationData = new AreaCreationData(m_viewPlan, point);
                    if (null != areaCreationData)
                    {
                        areaCreationData.TagPoint = tagPoint;
                        areaCreationDatas.Add(areaCreationData);
                    }
                }

                //Create areas
                if (0 == areaCreationDatas.Count)
                {
                    return false;
                }
                m_doc.Create.NewAreas(areaCreationDatas);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Batch creation of Columns
        /// </summary>
        /// <returns></returns>
        private bool CreateColumns()
        {
            try
            {
                List<FamilyInstanceCreationData> fiCreationDatas = new List<FamilyInstanceCreationData>();
                if (null == m_level)
                {
                    return false;
                }
                //Try to get a FamilySymbol
                FamilySymbol familySymbol = (from elem in
                                                 new FilteredElementCollector(m_doc)
                                                     .OfCategory(BuiltInCategory.OST_StructuralColumns)
                                                     .OfType<FamilySymbol>()
                                             let var = elem as FamilySymbol
                                             where var != null && var.Category != null &&
                                                 "Structural Columns" == var.Category.Name
                                             select var).First();
                if (null == familySymbol)
                {
                    return false;
                }

                if (!familySymbol.IsActive)
                {
                    familySymbol.Activate();
                }

                //Create FamilyInstanceCreationData for FamilyInstance's batch creation
                for (int i = 1; i < 11; i++)
                {
                    XYZ location = new XYZ(10, 100, 0);
                    m_doc.Create.NewFamilyInstance(location, familySymbol, m_level, StructuralType.Column);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Batch creation of Rooms
        /// </summary>
        /// <returns></returns>
        private bool CreateRooms()
        {
            try
            {
                if (null == m_level)
                {
                    return false;
                }

                //try to get Phase used to create rooms
                Phase phase = (from elem in
                                   new FilteredElementCollector(m_doc).OfClass(typeof(Phase)).ToElements()
                               select elem).First() as Phase;
                if (null == phase)
                {
                    return false;
                }
                for (int i = 1; i < 6; i++)
                {
                    UV point = new UV(i * 10, 0);
                    UV tagPoint = new UV(i * 10, 0);
                    Room room = m_doc.Create.NewRoom(m_level, point);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// batch creation of TextNotes
        /// </summary>
        /// <returns></returns>
        private bool CreateTextNotes()
        {
            try
            {
                //try to get View named "Level 1" where the TextNotes are
                View view = (from elem in
                                 new FilteredElementCollector(m_doc).OfClass(typeof(ViewPlan)).ToElements()
                             let var = elem as View
                             where var != null && !var.IsTemplate && null != var.Name && "Level 1" == var.Name &&
                                 ViewType.FloorPlan == var.ViewType
                             select var).First();
                if (null == view)
                {
                    return false;
                }

                //Create TextNotes
                ElementId typeId = m_doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType);

                for (int i = 1; i < 6; i++)
                {
                    XYZ origin = new XYZ(i * -20, -100, 0);
                    if (null == TextNote.Create(m_doc, view.Id, origin, "TextNote", typeId))
                    {
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        ///  Batch creation of Walls
        /// </summary>
        /// <returns></returns>
        private bool CreateWalls()
        {
            try
            {
                if (null == m_level)
                {
                    return false;
                }

                Application appCreation = m_cmdData.Application.Application.Create;
                for (int i = 1; i < 11; i++)
                {
                    //Create wall's profile which is a combine of rectangle and arc
                    IList<Curve> curveArray = new List<Curve>();
                    //create three lines for rectangle part of profile.
                    Autodesk.Revit.DB.XYZ pt1 = new Autodesk.Revit.DB.XYZ(i * 10, -80, 15);
                    Autodesk.Revit.DB.XYZ pt2 = new Autodesk.Revit.DB.XYZ(i * 10, -80, 0);
                    Autodesk.Revit.DB.XYZ pt3 = new Autodesk.Revit.DB.XYZ(i * 10, -90, 0);
                    Autodesk.Revit.DB.XYZ pt4 = new Autodesk.Revit.DB.XYZ(i * 10, -90, 15);

                    Line line1 = Line.CreateBound(pt1, pt2);
                    Line line2 = Line.CreateBound(pt2, pt3);
                    Line line3 = Line.CreateBound(pt3, pt4);

                    //create arc part of profile
                    XYZ pointInCurve = new XYZ(i * 10, -85, 20);
                    Arc arc = Arc.Create(pt4, pt1, pointInCurve);

                    if (null == line1 || null == line2 || null == line3 || null == arc)
                    {
                        continue;
                    }
                    curveArray.Add(line1);
                    curveArray.Add(line2);
                    curveArray.Add(line3);
                    curveArray.Add(arc);
                    Wall.Create(m_doc, curveArray, true);
                }

                for (int i = 1; i < 11; i++)
                {
                    Autodesk.Revit.DB.XYZ pt1 = new Autodesk.Revit.DB.XYZ(i * 10, -110, 0);
                    Autodesk.Revit.DB.XYZ pt2 = new Autodesk.Revit.DB.XYZ(i * 10, -120, 0);
                    Line line = Line.CreateBound(pt1, pt2);
                    Wall.Create(m_doc, line, m_level.Id, true);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}