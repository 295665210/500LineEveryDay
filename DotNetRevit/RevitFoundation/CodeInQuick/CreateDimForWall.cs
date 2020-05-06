using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitFoundation.CodeInQuick
{
    [Transaction(TransactionMode.Manual)]
    internal class CreateDimForWall : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            var app = uiapp.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;

            View activeView = uidoc.ActiveView;

            FilteredElementCollector filteredElementCollector = new FilteredElementCollector(doc, activeView.Id);

            ElementCategoryFilter elementCategoryFilter = new ElementCategoryFilter(BuiltInCategory.OST_Walls);

            ElementCategoryFilter elementCategoryFilter2 =
                new ElementCategoryFilter(BuiltInCategory.OST_WallsStructure);

            LogicalOrFilter logicalOrFilter = new LogicalOrFilter(elementCategoryFilter, elementCategoryFilter2);
            filteredElementCollector.WherePasses(logicalOrFilter).WhereElementIsNotElementType();

            double num = 3.2808398950131235;
            foreach (Element element in filteredElementCollector)
            {
                Wall wall = element as Wall;
                Location location = wall.Location;
                LocationCurve locationCurve = location as LocationCurve;
                Curve curve = locationCurve.Curve;

                if (curve is Line)
                {
                    Line line = curve as Line;
                    XYZ endPoint = line.GetEndPoint(0);
                    XYZ endPoint2 = line.GetEndPoint(1);
                    XYZ direction = line.Direction;
                    XYZ xyz = line.Direction.CrossProduct(activeView.ViewDirection);
                    try
                    {
                        Reference reference = this.GetReference(wall, -direction);
                        Reference reference2 = this.GetReference(wall, direction);
                        if (reference != null && reference2 != null)
                        {
                            ReferenceArray referenceArray = new ReferenceArray();
                            referenceArray.Append(reference);
                            referenceArray.Append(reference2);
                            Line line2 = Line.CreateBound(endPoint + num * xyz.Normalize(),
                                endPoint2 + num * xyz.Normalize());

                            TransactionGroup tsGroup = new TransactionGroup(doc, "直线的墙标注尺寸");
                            tsGroup.Start();
                            Transaction ts = new Transaction(doc, "dim");
                            ts.Start();
                            doc.Create.NewDimension(activeView, line2, referenceArray);
                            ts.Commit();
                            tsGroup.Assimilate();
                        }
                    }
                    catch (Exception e)
                    {
                        return Result.Succeeded;
                    }
                }
            }
            return Result.Succeeded;
        }


        private Reference GetReference(Wall wall, XYZ dir)
        {
            Reference result = null;
            GeometryElement geometryElement = wall.get_Geometry(new Options()
                {ComputeReferences = true, DetailLevel = ViewDetailLevel.Fine});
            foreach (GeometryObject geometryObject in geometryElement)
            {
                GeometryInstance geometryInstance = geometryObject as GeometryInstance;
                if (geometryInstance != null)
                {
                    GeometryElement symbolGeometry = geometryInstance.GetSymbolGeometry();
                    using (IEnumerator<GeometryObject> eunmerator2 = symbolGeometry.GetEnumerator())
                    {
                        while (eunmerator2.MoveNext())
                        {
                            GeometryObject geometryObject2 = eunmerator2.Current;
                            Solid solid = geometryObject as Solid;
                            if (solid != null && solid.Faces.Size > 0)
                            {
                                foreach (object obj in solid.Faces)
                                {
                                    Face face = (Face) obj;
                                    PlanarFace planarFace = face as PlanarFace;
                                    ;
                                    if (planarFace != null && planarFace.FaceNormal.IsAlmostEqualTo(dir))
                                    {
                                        result = planarFace.Reference;
                                        break;
                                    }
                                }
                            }
                        }
                        continue;
                    }
                }
                Solid solid2 = geometryObject as Solid;
                if (solid2 != null && solid2.Faces.Size > 0)
                {
                    foreach (object obj2 in solid2.Faces)
                    {
                        Face face2 = (Face) obj2;
                        PlanarFace planarFace2 = face2 as PlanarFace;
                        if (planarFace2 != null && planarFace2.FaceNormal.IsAlmostEqualTo(dir))
                        {
                            result = planarFace2.Reference;
                            break;
                        }
                    }
                }
            }
            return result;
        }
    }
}