using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CodeInBuildingCoder1
{
    [Transaction(TransactionMode.Manual)]
    class CmdCurtainWallGeom : IExternalCommand
    {
        /// <summary>
        /// GetElementSolids dummy place holder function.
        /// The real one would retrieve all solids from
        /// the given element geometry.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        List<Solid> GetElementSolids(Element e)
        {
            return null;
        }

        /// <summary>
        /// Get curtain wall panel geometry retrieves all solids
        /// from a curtain wall, including Basic panel walls.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="curtainWallId"></param>
        /// <param name="solids"></param>
        void GetCurtainWallPanelGeometry(
            Document doc, ElementId curtainWallId, List<Solid> solids)
        {
            Wall wall = doc.GetElement(curtainWallId) as Wall;
            var grid = wall.CurtainGrid;

            foreach (ElementId id in grid.GetPanelIds())
            {
                Element e = doc.GetElement(id);
                solids.AddRange(GetElementSolids(e));
            }

            FilteredElementCollector cwPanels =
                new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_CurtainWallPanels)
                    .OfClass(typeof(Wall));

            foreach (Wall cwp in cwPanels)
            {
                if (cwp.StackedWallOwnerId ==curtainWallId)
                {
                    solids.AddRange(GetElementSolids(cwp));
                }
            }
        }


        void list_wall_geom(Wall w, Application app)
        {
            string s = "";
            CurtainGrid cgrid = w.CurtainGrid;

            Options options = app.Create.NewGeometryOptions();
            options.ComputeReferences = true;
            options.IncludeNonVisibleObjects = true;

            GeometryElement geomElem = w.get_Geometry(options);

            foreach (GeometryObject obj in geomElem)
            {
                Visibility vis = obj.Visibility;
                string visString = vis.ToString();
                Arc arc = obj as Arc;
                Line line = obj as Line;
                Solid solid = obj as Solid;

                if (arc!= null)
                {
                    double length = arc.ApproximateLength;
                    s += "Length (arc) (" + visString + "): " + length + "\n";
                }
                if (line!=null)
                {
                    double length = line.ApproximateLength;
             
                    s += "Length (line) (" + visString + "): " + length + "\n";
                }

                if (solid !=null)
                {
                    
                }
            }

        }

        public Result Execute(
            ExternalCommandData commandData, ref string message,
            ElementSet elements)
        {
            throw new NotImplementedException();
        }
    }
}