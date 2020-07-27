using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;

namespace DisplacementElementAnimation
{
    class DisplacementStructureModelAnimator
    {
        public void StartAnimation()
        {
            //初始化动画的参数
            m_displacementElements = new List<DisplacementElement>();
            m_currentDisplacementIndex = 0;

            UIDocument uiDoc = uiApplication.ActiveUIDocument;
            View view = uiDoc.ActiveView;
            Document doc = uiDoc.Document;

            //get levels in order of elevation
            FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(Level));
            List<ICollection<ElementId>> idGroupsInOrder = new List<ICollection<ElementId>>();
            IEnumerable<Level> levels = collector.Cast<Level>().OrderBy<Level, double>(lvl => lvl.Elevation);

            //create  lists of "elements on level" in ascending(升序) order：
            //Foundation
            //Framing
            //Floors
            //walls 
            //columns

            foreach (Level level in levels)
            {
                AddInstancesOnLevelToIdGroupList(idGroupsInOrder, level, BuiltInCategory.OST_StructuralFoundation);
                AddInstancesOnReferenceLevelToIdGroupList(idGroupsInOrder, level,BuiltInCategory.OST_StructuralFraming);
                AddInstancesOnLevelToIdGroupList(idGroupsInOrder, level, BuiltInCategory.OST_Floors);
                AddInstancesOnLevelToIdGroupList(idGroupsInOrder, level, BuiltInCategory.OST_Walls);
                AddInstancesOnLevelToIdGroupList(idGroupsInOrder, level, BuiltInCategory.OST_StructuralColumns);
            }

            //Initial setup of displacement elements of animation
            using (Transaction t = new Transaction(doc, "start animation"))
            {
                t.Start();
                foreach (ICollection<ElementId> idGroups in idGroupsInOrder)
                {
                    BuildDisplacementElementGroup(doc, idGroups, view);
                }
                if (m_displacementElements.Count == 0)
                {
                    t.RollBack();
                    return;
                }
                m_displacementElement = m_displacementElements[0];
                UnhideDisplacedElements();
                t.Commit();
            }

            displacementParameter = 1.0;

            if (m_isUsingIdling)
            {
                uiApplication.Idling += IdlingResponse;


                m_timer = new System.Timers.Timer();
                //时间间隔
                m_timer.Interval = timerInterval;
                m_timer.Elapsed += TimerElapsed;
                m_timer.Start();
            }
        }


        public void AnimateNextStep()
        {
            bool groupFinished = false; // Is the current animation group finished?
            bool allFinished = false;   //are all animation groups finished?
            if (displacementParameter <= 0)
            {
                displacementParameter = 1.0;
                groupFinished = true;
                m_currentDisplacementIndex++;
                if (m_currentDisplacementIndex == m_displacementElements.Count)
                {
                    allFinished = true;
                    if (m_isUsingIdling)
                    {
                        m_timer.Stop();
                        uiApplication.Idling -= IdlingResponse;
                    }
                }
            }

            using (Transaction t = new Transaction(uiApplication.ActiveUIDocument.Document,
                groupFinished ? "Next animation group" : "Animation step"))
            {
                t.Start();
                if (groupFinished)
                {
                    // Delete displacement element (and children)
                    uiApplication.ActiveUIDocument.Document.Delete(m_displacementElement.Id);

                    // Increment to next group
                    m_displacementElement = allFinished ? null : m_displacementElements[m_currentDisplacementIndex];
                    UnhideDisplacedElements();
                }
                else
                {
                    ChangeDisplacementLocationForParent();
                    ChangeDisplacementLocationsForChildren();

                    // Decrement displacement parameter
                    displacementParameter -= displacementIncrement;
                }
                t.Commit();
            }
        }

        private void UnhideDisplacedElements()
        {
            if (m_displacementElements != null)
            {
                View view = m_displacementElement.Document.GetElement(m_displacementElement.OwnerViewId) as View;
                view.UnhideElements(m_displacementElement.GetDisplacedElementIdsFromAllChildren());
            }
        }

        /// Find all instances matching category and level, and add to the collection of groups of sorted ids.
        private static void AddInstancesOnLevelToIdGroupList
            (List<ICollection<ElementId>> idGroupsInOrder, Level level, BuiltInCategory category)
        {
            FilteredElementCollector collector = new FilteredElementCollector(level.Document);
            collector.WherePasses(new ElementLevelFilter(level.Id));
            collector.OfCategory(category);
            collector.WhereElementIsNotElementType();
            ICollection<ElementId> idGroup = collector.ToElementIds();
            //only add non-empty groups
            if (idGroup.Count > 0)
            {
                idGroupsInOrder.Add(idGroup);
            }
        }

        private void AddInstancesOnReferenceLevelToIdGroupList
            (List<ICollection<ElementId>> idGroupsInOrder, Level level, BuiltInCategory category)
        {
            FilteredElementCollector collector = new FilteredElementCollector(level.Document);
            collector.OfCategory(category);
            collector.WhereElementIsNotElementType();
            // use a parameter filter to match the reference level parameter
            FilterRule rule =
                ParameterFilterRuleFactory.CreateEqualsRule(
                    new ElementId(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM), level.Id);
            collector.WherePasses(new ElementParameterFilter(rule));
            ICollection<ElementId> idGroup = collector.ToElementIds();
            //only add non-empty groups
            if (idGroup.Count > 0)
            {
                idGroupsInOrder.Add(idGroup);
            }
        }

        private void BuildDisplacementElementGroup(Document doc, ICollection<ElementId> ids, View view)
        {
            Element lastElement = doc.GetElement(ids.Last<ElementId>());
            XYZ parentDisplacedLocation = GetDisplacementXYFor(lastElement, XYZ.Zero);
            parentDisplacedLocation = MoveToElevation(parentDisplacedLocation, initialHeight);

            //all elements are added to the parent displacement element.
            DisplacementElement parent = DisplacementElement.Create(doc, ids, parentDisplacedLocation, view, null);
            m_displacementElements.Add(parent);
            int count = ids.Count;
            List<ElementId> childIds = new List<ElementId>();
            List<ElementId> idsList = ids.ToList<ElementId>();

            //add all elements except the last one to child displacement elements
            for (int index = 0; index < count - 1; index++)
            {
                ElementId childId = idsList[index];
                Element e = doc.GetElement(childId);
                XYZ displacedLocation = GetDisplacementXYFor(e, parentDisplacedLocation);

                //setup id container for child DisplacementElement creation
                childIds.Clear();
                childIds.Add(childId);
                DisplacementElement child = DisplacementElement.Create(doc, childIds, displacedLocation, view, parent);
            }
            view.HideElements(ids);
        }

        private void ChangeDisplacementLocationForParent()
        {
            //displacement include displacement in z
            XYZ displacement = GetDisplacementXY(m_displacementElement);
            displacement = MoveToElevation(displacement, GetHeightDisplacementValue());
            m_displacementElement.SetRelativeDisplacement(displacement);
        }

        private void ChangeDisplacementLocationsForChildren()
        {
            IList<DisplacementElement> subDisplacementElements = m_displacementElement.GetChildren();
            foreach (DisplacementElement element in subDisplacementElements)
            {
                //displacement does not include change in z
                XYZ displacedLocation = GetDisplacementXY(element);
                element.SetRelativeDisplacement(displacedLocation);
            }
        }

        private static bool DisplacementElementIsChild(DisplacementElement element)
        {
            return element.ParentId != ElementId.InvalidElementId;
        }


        private XYZ GetDisplacementXY(DisplacementElement element)
        {
            //if element is a child ,need to take into account the displacement of the parent.
            XYZ displacementDueToParent = XYZ.Zero;
            if (DisplacementElementIsChild(element))
            {
                DisplacementElement parent = element.Document.GetElement(element.ParentId) as DisplacementElement;
                displacementDueToParent = parent.GetRelativeDisplacement();
            }
            //assume one element per displacement, use the element to get the displacement needed.
            ElementId id = element.GetDisplacedElementIds().First<ElementId>();
            Element e = element.Document.GetElement(id);

            return GetDisplacementXYFor(e, displacementDueToParent);
        }

        private XYZ GetDisplacementXYFor(Element e, XYZ displacementDueToParent)
        {
            XYZ displacementDueToParentXY = MoveToElevationZero(displacementDueToParent);
            XYZ location = GetNominalCenterLocation(e);
            XYZ delta = (location - modelCenter);
            XYZ displacedLocation = delta * GetXYDisplacementRatio() - displacementDueToParentXY;
            return displacedLocation;
        }


        private double GetXYDisplacementRatio()
        {
            if (displacementParameter == 1.0)
                return initialXYRatio;
            return initialXYRatio * 1 / Math.Pow(initialHeight - GetHeightDisplacementValue(), 0.75);
        }


        private double GetHeightDisplacementValue()
        {
            return initialHeight * displacementParameter;
        }


        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            timerTripped = true;
        }

        private void IdlingResponse(object sender, IdlingEventArgs e)
        {
            e.SetRaiseWithoutDelay();
            if (timerTripped)
            {
                timerTripped = false;

                AnimateNextStep();
            }
        }

        public DisplacementStructureModelAnimator(UIApplication uiapp, bool isUsingIdling)
        {
            uiApplication = uiapp;
            m_isUsingIdling = isUsingIdling;
        }


        private static XYZ MoveToElevationZero(XYZ location)
        {
            return MoveToElevation(location, 0);
        }

        private static XYZ MoveToElevation(XYZ location, double z)
        {
            return new XYZ(location.X, location.Y, z);
        }


        private XYZ GetNominalCenterLocation(Element e)
        {
            LocationPoint lp = e.Location as LocationPoint;
            if (lp != null)
            {
                return MoveToElevationZero(lp.Point);
            }

            LocationCurve lc = e.Location as LocationCurve;
            if (lc != null)
            {
                return MoveToElevationZero(lc.Curve.Evaluate(0.5, true));
            }

            return XYZ.Zero;
        }


        private UIApplication uiApplication;

        private List<DisplacementElement> m_displacementElements;

        private DisplacementElement m_displacementElement = null;

        private int m_currentDisplacementIndex = 0;

        private double displacementParameter = 1.0;

        private double displacementIncrement = 0.05;

        private double initialXYRatio = 1.25;

        private double initialHeight = 100;

        private bool timerTripped = false;


        private int timerInterval = 60;

        private System.Timers.Timer m_timer = null;

        private XYZ modelCenter = XYZ.Zero;

        private bool m_isUsingIdling = true;
    }
}