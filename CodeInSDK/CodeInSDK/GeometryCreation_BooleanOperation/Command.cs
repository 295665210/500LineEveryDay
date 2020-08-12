using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;
using Autodesk.Revit.UI;
using CodeInSDK.GeometryCreation_BooleanOperation.CS;

namespace CodeInSDK.GeometryCreation_BooleanOperation
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                Autodesk.Revit.DB.Document document = commandData.Application.ActiveUIDocument.Document;
                //create a new transaction
                Transaction tran = new Transaction(document, "GeometryCreation_BooleanOperation");
                tran.Start();

                //create an object that is responsible for creating the solids
                GeometryCreation geometryCreation = GeometryCreation.getInstance(commandData.Application.Application);

                //create an object that is responsible for displaying the solids
                AnalysisVisualizationFramework AVF = AnalysisVisualizationFramework.getInstance(document);

                //create a CSG tree solid
                CSGTree(geometryCreation, AVF);

                tran.Commit();

                //set the view which display the solid active
                commandData.Application.ActiveUIDocument.ActiveView = (((
                    new FilteredElementCollector(document)
                        .OfClass(typeof(View)))
                    .Cast<View>())
                    .Where(e => e.Name == "CSGTree"))
                    .First<View>();

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return Result.Failed;
            }
        }

        /// <summary>
        /// Create a constructive solid geometry - CSG tree.
        /// </summary>
        /// <param name="geometryCreation"></param>
        /// <param name="avf"></param>
        private void CSGTree(GeometryCreation geometryCreation, AnalysisVisualizationFramework avf)
        {
            List<Solid> materialSolids = prepareSolids(geometryCreation);

            //operation 1 
            Solid CSGTree_solid1 = BooleanOperation.BooleanOperation_Intersect(materialSolids[0], materialSolids[1]);

            //operation 2
            Solid CSGTree_solid2 = BooleanOperation.BooleanOperation_Union(materialSolids[2], materialSolids[3]);

            //operation 3
            BooleanOperation.BooleanOperation_Union(ref CSGTree_solid2, materialSolids[4]);

            //operation 4
            BooleanOperation.BooleanOperation_Difference(ref CSGTree_solid1, CSGTree_solid2);

            avf.PaintSolid(CSGTree_solid1, "CSGTree");
        }

        /// <summary>
        /// Create five solids materials for CSG tree.
        /// </summary>
        /// <param name="geometryCreation"></param>
        /// <returns></returns>
        private List<Solid> prepareSolids(GeometryCreation geometrycreation)
        {
            List<Solid> resultSolids = new List<Solid>();

            resultSolids.Add(geometrycreation.CreateCenterbasedBox(Autodesk.Revit.DB.XYZ.Zero, 25));

            resultSolids.Add(geometrycreation.CreateCenterbasedSphere(Autodesk.Revit.DB.XYZ.Zero, 20));

            resultSolids.Add(geometrycreation.CreateCenterbasedCylinder(Autodesk.Revit.DB.XYZ.Zero, 5, 40, 
                GeometryCreation.CylinderDirection.BasisX));

            resultSolids.Add(geometrycreation.CreateCenterbasedCylinder(Autodesk.Revit.DB.XYZ.Zero, 5, 40, 
                GeometryCreation.CylinderDirection.BasisY));

            resultSolids.Add(geometrycreation.CreateCenterbasedCylinder(Autodesk.Revit.DB.XYZ.Zero, 5, 40, 
                GeometryCreation.CylinderDirection.BasisZ));

            return resultSolids;
        }
    }
}