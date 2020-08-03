using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace DirectionCalculation
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class FindSouthFacingWalls : FindSouthFacingBase
    {
        protected void Execute(bool useProjectLocationNorth)
        {
            UIDocument uiDoc = new UIDocument(Document);
            ElementSet selElements = new ElementSet();
            foreach (ElementId elementId in uiDoc.Selection.GetElementIds())
            {
                selElements.Insert(uiDoc.Document.GetElement(elementId));
            }
            IEnumerable<Wall> walls = CollectExteriorWalls();
        }

        /// <summary>
        ///  Finds all exterior walls in the active document.
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<Wall> CollectExteriorWalls()
        {
            FilteredElementCollector collector = new FilteredElementCollector(Document);
            IList<Element> elementsToProcess = collector.OfClass(typeof(Wall)).ToElements();
            //use a LINQ query to filter only exterior walls
            IEnumerable<Wall> exteriorWalls =
                from wall in elementsToProcess.Cast<Wall>()
                where IsExterior(Document.GetElement(wall.GetTypeId()) as ElementType)
                select wall;
            return exteriorWalls;
        }

        /// <summary>
        /// test method to see if the wall type is exterior.
        /// </summary>
        /// <param name="re"></param>
        /// <returns></returns>
        protected bool IsExterior(ElementType wallType)
        {
            Parameter wallFunction = wallType.get_Parameter(BuiltInParameter.FUNCTION_PARAM);
            WallFunction value = (WallFunction) wallFunction.AsInteger();
            return (value == WallFunction.Exterior);
        }


        protected XYZ GetExteriorWallDirection(Wall wall)
        {
            LocationCurve locationCurve = wall.Location as LocationCurve;
            XYZ exteriorDirection = XYZ.BasisZ;

            if (locationCurve != null)
            {
                Curve curve = locationCurve.Curve;
                //Write("Wall line endpoints :" ,curve );
                XYZ direction = XYZ.BasisX;
                if (curve is Line)
                {
                    //obtains(获得) the tangent vector of the wall.
                    direction = curve.ComputeDerivatives(0, true).BasisX.Normalize();
                }
                else
                {
                    //An assumption, for non-linear walls, that the (tangent vector正切向量)is the direction
                    //from the start of the wall to the end.
                    direction = (curve.GetEndPoint(1) - curve.GetEndPoint(0)).Normalize();
                }

                //calculate the normal vector via cross product.
                exteriorDirection = XYZ.BasisZ.CrossProduct(direction);

                //Flipped walls need to reverse the calculated direction
                if (wall.Flipped)
                {
                    exteriorDirection = -exteriorDirection;
                }
            }
            return exteriorDirection;
        }
    }
}