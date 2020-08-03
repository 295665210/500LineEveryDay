using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Form = Autodesk.Revit.DB.Form;

namespace DirectionCalculation
{
    public class FindSouthFacingWindows : FindSouthFacingBase
    {
        protected void Execute(bool useProjectLocationNorth)
        {
            UIDocument uiDoc = new UIDocument(Document);
            ElementSet selElements = new ElementSet();
            foreach (ElementId elementId in uiDoc.Selection.GetElementIds())
            {
                selElements.Insert(uiDoc.Document.GetElement(elementId));
            }

            IEnumerable<FamilyInstance> windows = CollectWindows();
            foreach (FamilyInstance window in windows)
            {
                XYZ exteriorDirection = GetWindowDirection(window);

                if (useProjectLocationNorth)
                {
                    exteriorDirection = TransformByProjectLocation(exteriorDirection);
                }

                bool isSouthFacing = IsSouthFacing(exteriorDirection);
                if (isSouthFacing)
                {
                    selElements.Insert(window);
                }
            }
            //select all windows which had the proper direction
            List<ElementId> elemIdList = new List<ElementId>();
            foreach (Element element in selElements)
            {
                elemIdList.Add(element.Id);
            }

            uiDoc.Selection.SetElementIds(elemIdList);
        }

        private XYZ GetWindowDirection(FamilyInstance window)
        {
            Options options = new Options();
            //extract the geometry of the window.
            GeometryElement geomElem = window.get_Geometry(options);
            //foreach 
            IEnumerator<GeometryObject> Objects = geomElem.GetEnumerator();
            while (Objects.MoveNext())
            {
                GeometryObject geomObj = Objects.Current;
                //we expect there to be one main Instance in each window. Ignore the rest of the geometry.
                GeometryInstance instance = geomObj as GeometryInstance;
                if (instance != null)
                {
                    //obtain the Instance's transform and nominal facing direction(Y-direction)
                    Transform t = instance.Transform;
                    XYZ facingDirection = t.BasisY;

                    //if the window is flipped in one direction ,but not the other,the transform is left handed.
                    // the Y direction need to be reversed to obtain the facing direction.
                    if ((window.FacingFlipped && !window.HandFlipped) || (!window.FacingFlipped && window.HandFlipped))
                    {
                        facingDirection = -facingDirection;
                    }
                    return facingDirection;
                }
            }
            return XYZ.BasisZ;
        }

        private IEnumerable<FamilyInstance> CollectWindows()
        {
            //windows are family instances whose category is correctly set.
            ElementClassFilter familyInstanceFilter = new ElementClassFilter(typeof(FamilyInstance));
            ElementCategoryFilter windowCategoryFilter = new ElementCategoryFilter(BuiltInCategory.OST_Windows);
            LogicalAndFilter andFilter = new LogicalAndFilter(familyInstanceFilter, windowCategoryFilter);
            FilteredElementCollector collector = new FilteredElementCollector(Document);
            ICollection<Element> elementsToProcess = collector.WherePasses(andFilter).ToElements();
            //用LINQ试试
            // ICollection<Element> elementsToProcess2 = new FilteredElementCollector(Document)
            //     .OfCategory(BuiltInCategory.OST_Windows).OfClass(typeof(FamilyInstance)).ToElements();
            // if (elementsToProcess == elementsToProcess2)
            // {
            //     MessageBox.Show("两种过滤方式得到的结果一致");
            // }
            //convert to IEnumerable of FamilyInstance using LINQ
            IEnumerable<FamilyInstance> windows = from window in elementsToProcess.Cast<FamilyInstance>()
                                                  select window;

            return windows;
        }

        protected XYZ GetWindowDirectionAlternate(FamilyInstance window)
        {
            return window.FacingOrientation;
        }
    }
}