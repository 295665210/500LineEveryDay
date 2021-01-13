using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.Exceptions;
using Autodesk.Revit.UI;
using ArgumentException = System.ArgumentException;


namespace CodeInBuildingCoder1
{
    [Transaction(TransactionMode.Manual)]
    class CmdColumnRound : IExternalCommand
    {
        /// <summary>
        /// Determine the height of a vertical column from its top
        /// and bottom level.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public double GetColumnHeightFromLevels(Element e)
        {
            if (!IsColumn(e))
            {
                throw new ArgumentException("Expected a column argument.");
            }

            Document doc = e.Document;
            double height = 0;
            if (e != null)
            {
                //Get top level of the column
                Parameter topLevel =
                    e.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM);
                ElementId ip = topLevel.AsElementId();
                Level top = doc.GetElement(ip) as Level;
                double t_value = top.ProjectElevation;

                //Get base level of the column.
                Parameter botLevel =
                    e.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_PARAM);
                ElementId bip = botLevel.AsElementId();
                Level bot = doc.GetElement(bip) as Level;
                double b_value = bot.ProjectElevation;

                height = t_value - b_value;
            }
            return height;
        }

        /// <summary>
        /// Determine the height of any given element
        /// from its bounding box. 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public double GetElementHeightFromBoundingBox(Element e)
        {
            //No need to retrieve the full element geometry.
            //Even if there were, there would be no need to 
            //compute references,because they will not be
            //used anyway.
            BoundingBoxXYZ bb = e.get_BoundingBox(null);
            if (null == bb)
            {
                throw new
                    ArgumentException("Expected Element 'e' to have a valid bounding box.");
            }
            return bb.Max.Z - bb.Min.Z;
        }

        bool IsColumn(Element e)
        {
            return e is FamilyInstance && null != e.Category &&
                   e.Category.Name.ToLower().Contains("柱");
        }

        public Result Execute(
            ExternalCommandData commandData, ref string message,
            ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            Result rc = Result.Failed;

            Element column =
                Util.SelectSingleElementOfType(uidoc, typeof(FamilyInstance),
                                               "column", true);
            if (null == column || !IsColumn(column))
            {
                message = "Please select a single column instance.";
            }
            else
            {
                Options opt = uiapp.Application.Create.NewGeometryOptions();
                GeometryElement geo = column.get_Geometry(opt);
                GeometryInstance i = null;
                foreach (GeometryObject obj in geo)
                {
                    i = obj as GeometryInstance;
                    if (null != i)
                    {
                        break;
                    }
                }
                if (null == i)
                {
                    message = "Unable to obtain geometry instance.";
                }
                else
                {
                    bool isCylindrical = false;
                    geo = i.SymbolGeometry;
                    foreach (GeometryObject obj in geo)
                    {
                        Solid solid = obj as Solid;
                        if (null != solid)
                        {
                            foreach (Face face in solid.Faces)
                            {
                                if (face is CylindricalFace)
                                {
                                    isCylindrical = true;
                                    break;
                                }
                            }
                        }
                    }
                    message =
                        $"Selected column instance is {(isCylindrical ? "" : "Not")} cylindrical";
                }
                TaskDialog.Show("Info", message);
                rc = Result.Succeeded;
            }
            return rc;
        }
    }
}