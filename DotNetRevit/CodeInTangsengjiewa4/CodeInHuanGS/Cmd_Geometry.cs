using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CodeInTangsengjiewa4.CodeInHuanGS
{
    /// <summary>
    ///  宦国胜书3.7章 几何
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_Geometry : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            var app = uiapp.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;

            //3-50 : 创建 Geometry.Options 
            Options geomOptions = app.Create.NewGeometryOptions();
            if (null != geomOptions)
            {
                geomOptions.ComputeReferences = true;
                geomOptions.DetailLevel = ViewDetailLevel.Fine;
                TaskDialog.Show("revit", "Geometry Option created successfully.");
            }

            return Result.Succeeded;
        }

        private void GetFacesAndEdges(Wall wall)
        {
            string faceInfo = "";
            Options opt = new Options();
            GeometryElement geomElem = wall.get_Geometry(opt);
            foreach (GeometryObject geometryObject in geomElem)
            {
                Solid geomSolid = geometryObject as Solid;
                if (null != geomSolid)
                {
                    int faces = 0;
                    double totalArea = 0;
                    foreach (Face geomFace in geomSolid.Faces)
                    {
                        faces++;
                        faceInfo += "Face" + faces + "area:" + geomFace.Area.ToString() + "\n";
                        totalArea += geomFace.Area;
                    }
                    faceInfo += "Number of faces" + faces + "\n";
                    faceInfo += "Total area :" + totalArea.ToString() + "\n";
                    foreach (Edge geomEdge in geomSolid.Edges)
                    {
                        //get wall's geometry edges
                    }
                }
            }
            TaskDialog.Show("revit", faceInfo);
        }
    }
}