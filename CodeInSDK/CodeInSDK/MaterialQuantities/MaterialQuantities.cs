using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Converters;
using Autodesk.Private.InfoCenterLib;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CodeInSDK.MaterialQuantities
{
    /// <summary>
    /// Outputs an analysis of the materials that make up walls, floors, and roofs ,and displays the output in Excel.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        static AddInId appId = new AddInId(new Guid("7E5CAC0D-F3D8-4040-89D6-0828D681561B"));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Result Execute(ExternalCommandData revit, ref string message, ElementSet elements)
        {
            Autodesk.Revit.ApplicationServices.Application app = revit.Application.Application;
            m_doc = revit.Application.ActiveUIDocument.Document;

            String filename = "CalculateMaterialQuantities2.txt";

            m_writer = new StreamWriter(filename);

            ExecuteCalculationsWith<RoofMaterialQuantityCalculator>();
            ExecuteCalculationsWith<WallMaterialQuantityCalculator>();
            ExecuteCalculationsWith<FloorMaterialQuantityCalculator>();

            m_writer.Close();

            //this operation doesn't change the model, so return cancelled to cancel the transaction.
            return Result.Cancelled;
        }

        private void ExecuteCalculationsWith<T>() where T : MaterialQuantityCalculator, new()
        {
            T calculator = new T();
            calculator.SetDocument(m_doc);
            calculator.CalculateMaterialQuantities();
            calculator.ReportResults(m_writer);
        }

#region  basic command data
        private Document m_doc;
        private TextWriter m_writer;
#endregion
    }

    /// <summary>
    /// the wall material quantity calculator specialized class.
    /// </summary>
    class WallMaterialQuantityCalculator : MaterialQuantityCalculator
    {
        protected override void CollectElements()
        {
            FilteredElementCollector collector = new FilteredElementCollector(m_doc);
            m_elementsToProcess = collector.OfCategory(BuiltInCategory.OST_Walls).WhereElementIsNotElementType()
                .ToElements();
        }

        protected override string GetElementTypeName()
        {
            return "Wall";
        }
    }

    class FloorMaterialQuantityCalculator : MaterialQuantityCalculator
    {
        protected override void CollectElements()
        {
            FilteredElementCollector collector = new FilteredElementCollector(m_doc);
            m_elementsToProcess = collector.OfCategory(BuiltInCategory.OST_Floors).WhereElementIsNotElementType()
                .ToElements();
        }

        protected override string GetElementTypeName()
        {
            return "Floor";
        }
    }

    class RoofMaterialQuantityCalculator : MaterialQuantityCalculator
    {
        protected override void CollectElements()
        {
            FilteredElementCollector collector = new FilteredElementCollector(m_doc);
            m_elementsToProcess = collector.OfCategory(BuiltInCategory.OST_Roofs).WhereElementIsNotElementType()
                .ToElements();
        }

        protected override string GetElementTypeName()
        {
            return "Roof";
        }
    }


    abstract class MaterialQuantityCalculator
    {
        /// <summary>
        /// The list of elements for material quantity extraction
        /// </summary>
        protected IList<Element> m_elementsToProcess;

        /// <summary>
        /// Override this to populate the list of elements for material quantity extraction
        /// </summary>
        protected abstract void CollectElements();

        /// <summary>
        /// Override this to return the name of the element type calculated by this calculator.
        /// </summary>
        /// <returns></returns>
        protected abstract String GetElementTypeName();

        /// <summary>
        /// Sets the document for the calculator class.
        /// </summary>
        /// <param name="d"></param>
        public void SetDocument(Document d)
        {
            m_doc = d;
            Autodesk.Revit.ApplicationServices.Application app = d.Application;
        }

        /// <summary>
        /// Executes the calculation.
        /// </summary>
        public void CalculateMaterialQuantities()
        {
            CollectElements();
            CalculateNetMaterialQuantities();
            CalculateGrossMaterialQuantities();
        }

        /// <summary>
        /// Calculates gross material quantities for the target elements(material quantities with all openings, doors and windows removed.)
        /// </summary>
        private void CalculateGrossMaterialQuantities()
        {
            m_calculatingGrossQuantities = true;
            Transaction t = new Transaction(m_doc);
            t.SetName("Delete Cutting Elements");
            t.Start();
            DeleteAllCuttingElements();
            m_doc.Regenerate();
            foreach (Element e in m_elementsToProcess)
            {
                CalculateMaterialQuantitiesOfElement(e);
            }
            t.RollBack();
        }

        /// <summary>
        /// calculates net material quantities for the target elements.
        /// </summary>
        private void CalculateNetMaterialQuantities()
        {
            foreach (Element e in m_elementsToProcess)
            {
                CalculateMaterialQuantitiesOfElement(e);
            }
        }


        /// <summary>
        /// Delete all elements that cut out of target elements, to allow for calculation of gross material quantities.
        /// </summary>
        private void DeleteAllCuttingElements()
        {
            IList<ElementFilter> filterList = new List<ElementFilter>();
            FilteredElementCollector collector = new FilteredElementCollector(m_doc);

            ElementClassFilter filterFamilyInstance = new ElementClassFilter(typeof(FamilyInstance));
            ElementCategoryFilter filterWindowCategory = new ElementCategoryFilter(BuiltInCategory.OST_Windows);
            ElementCategoryFilter filterDoorCategory = new ElementCategoryFilter(BuiltInCategory.OST_Doors);
            LogicalOrFilter filterDoorOrWindowCategory = new LogicalOrFilter(filterWindowCategory, filterDoorCategory);

            LogicalAndFilter filterDoorWindowInstance =
                new LogicalAndFilter(filterDoorOrWindowCategory, filterFamilyInstance);

            ElementClassFilter filterOpening = new ElementClassFilter(typeof(Opening));

            LogicalOrFilter filterCuttingElements = new LogicalOrFilter(filterOpening, filterDoorWindowInstance);

            ICollection<Element> cuttingElementsList = collector.WherePasses(filterCuttingElements).ToElements();

            foreach (Element e in cuttingElementsList)
            {
                if (e.Category != null)
                {
                    if (e.Category.Id.IntegerValue == (int) BuiltInCategory.OST_Doors)
                    {
                        FamilyInstance door = e as FamilyInstance;
                        Wall host = door.Host as Wall;
                        if (host.CurtainGrid != null)
                        {
                            continue;
                        }
                    }

                    ICollection<ElementId> deletedElements = m_doc.Delete(e.Id);
                    //log failed deletion attempts to output(These may be other situations where deletion is not possible but the failure does not really affect the results.)
                    if (deletedElements == null || deletedElements.Count < 1)
                    {
                        m_warningsForGrossQuantityCalculations.Add(String.Format(
                            "  The tool wa unable to delete the {0} named {2} (id {1})", e.GetType().Name, e.Id,
                            e.Name));
                    }
                }
            }
        }

        /// <summary>
        /// Store calculated material quantities in the storage collection.
        /// </summary>
        /// <param name="materialId"></param>
        /// <param name="volume"></param>
        /// <param name="area"></param>
        /// <param name="quantities"></param>
        private void StoreMaterialQuantities
            (ElementId materialId, double volume, double area, Dictionary<ElementId, MaterialQuantities> quantities)
        {
            MaterialQuantities materialQuantityPerElement;
            bool found = quantities.TryGetValue(materialId, out materialQuantityPerElement);
            if (found)
            {
                if (m_calculatingGrossQuantities)
                {
                    materialQuantityPerElement.GrossVolume += volume;
                    materialQuantityPerElement.GrossArea += area;
                }
                else
                {
                    materialQuantityPerElement.NetVolume += volume;
                    materialQuantityPerElement.NetArea += area;
                }
            }
            else
            {
                materialQuantityPerElement = new MaterialQuantities();
                if (m_calculatingGrossQuantities)
                {
                    materialQuantityPerElement.GrossVolume += volume;
                    materialQuantityPerElement.GrossArea += area;
                }
                else
                {
                    materialQuantityPerElement.NetVolume += volume;
                    materialQuantityPerElement.NetArea += area;
                }
                quantities.Add(materialId, materialQuantityPerElement);
            }
        }

        /// <summary>
        /// Calculate and store material quantities for a given element.
        /// </summary>
        private void CalculateMaterialQuantitiesOfElement(Element e)
        {
            ElementId elementId = e.Id;
            ICollection<ElementId> materials = e.GetMaterialIds(false);

            foreach (ElementId materialId in materials)
            {
                double volume = e.GetMaterialVolume(materialId);
                double area = e.GetMaterialArea(materialId, false);

                if (volume > 0.0 || area > 0.0)
                {
                    StoreMaterialQuantities(materialId, volume, area, m_totalQuantities);

                    Dictionary<ElementId, MaterialQuantities> quantityPerElement;
                    bool found = m_quantitiesPerElement.TryGetValue(elementId, out quantityPerElement);
                    if (found)
                    {
                        StoreMaterialQuantities(materialId, volume, area, quantityPerElement);
                    }
                    else
                    {
                        quantityPerElement = new Dictionary<ElementId, MaterialQuantities>();
                        StoreMaterialQuantities(materialId, volume, area, quantityPerElement);
                        m_quantitiesPerElement.Add(elementId, quantityPerElement);
                    }
                }
            }
        }

        /// <summary>
        /// write results in CSV format to the indicated output writer
        /// </summary>
        /// <param name="writer"></param>
        public void ReportResults(TextWriter writer)
        {
            if (m_totalQuantities.Count == 0)
            {
                return;
            }

            String legendLine = "Gross volume(cubic ft),Net volume(cubic ft),Gross area(sq ft),Net area(sq ft)";

            writer.WriteLine();

            writer.WriteLine(String.Format("Totals for {0} elements, {1}", GetElementTypeName(), legendLine));

            //If unexpected deletion failures occured ,log the warning to the output.
            if (m_warningsForGrossQuantityCalculations.Count > 0)
            {
                writer.WriteLine(
                    "WARNING: Calculations for gross volume and area may not be completely accurate due to the following warnings:");
                foreach (string s in m_warningsForGrossQuantityCalculations)
                {
                    writer.WriteLine(s);
                    writer.WriteLine();
                }
            }
            ReportResultsFor(m_totalQuantities, writer);

            foreach (ElementId keyId in m_quantitiesPerElement.Keys)
            {
                ElementId id = keyId;
                Element e = m_doc.GetElement(id);

                writer.WriteLine();
                writer.WriteLine(String.Format("Totals for {0} element {1} (id {2}),{3}",GetElementTypeName(),e.Name.Replace(',', ':'), // Element names may have ',' in them
                    id.IntegerValue, legendLine));

                Dictionary<ElementId, MaterialQuantities> quantitieses = m_quantitiesPerElement[id];

                ReportResultsFor(quantitieses,writer);

            }
        }

        private void ReportResultsFor(Dictionary<ElementId, MaterialQuantities> quantities, TextWriter writer)
        {
            foreach (ElementId keyMaterialId in quantities.Keys)
            {
                ElementId materialId = keyMaterialId;
                MaterialQuantities quantity = quantities[materialId];

                Material material = m_doc.GetElement(materialId) as Material;

                writer.WriteLine(String.Format("{0},{3:F2},{1:F2},{4:F2},{2:F2}", material.Name.Replace(',', ':'),
                    quantity.NetVolume, quantity.NetArea, quantity.GrossVolume, quantity.GrossArea));
            }
        }


#region  Results Storage
        protected Document m_doc;

        private List<String> m_warningsForGrossQuantityCalculations = new List<string>();

        private bool m_calculatingGrossQuantities = false;

        private Dictionary<ElementId, MaterialQuantities> m_totalQuantities =
            new Dictionary<ElementId, MaterialQuantities>();

        private Dictionary<ElementId, Dictionary<ElementId, MaterialQuantities>> m_quantitiesPerElement =
            new Dictionary<ElementId, Dictionary<ElementId, MaterialQuantities>>();
#endregion
    }

    /// <summary>
    /// A storage class for the extracted material quantities.
    /// </summary>
    class MaterialQuantities
    {
        /// <summary>
        /// Gross volume( cubic ft)
        /// </summary>
        public double GrossVolume { get; set; }
        /// <summary>
        /// Gross area
        /// </summary>
        public double GrossArea { get; set; }
        /// <summary>
        /// Net volume(cubic ft)
        /// </summary>
        public double NetVolume { get; set; }
        /// <summary>
        /// Net area(sq. ft)
        /// </summary>
        public double NetArea { get; set; }
    }
}