// using System;
// using System.Collections.Generic;
// using System.Diagnostics;
// using System.Linq;
// using System.Security.Cryptography.X509Certificates;
// using System.Text;
// using System.Threading.Tasks;
// using Autodesk.Revit.ApplicationServices;
// using Autodesk.Revit.Attributes;
// using Autodesk.Revit.DB;
// using Autodesk.Revit.UI;
//
// // code in BuildingCoder
// namespace CodeInSDK.CmdDimensionWallsIterateFaces
// {
//     [Transaction(TransactionMode.Manual)]
//     class CmdDimensionWallsIterateFaces : IExternalCommand
//     {
// #region Developer Guide Sample Code
//         public void DuplicateDimension(Document doc, Dimension dimension)
//         {
//             Line line = dimension.Curve as Line;
//             if (null != line)
//             {
//                 View view = dimension.View;
//                 ReferenceArray references = dimension.References;
//                 Dimension newDimension = doc.Create.NewDimension(view, line, references);
//             }
//         }
//
//         public Dimension CreateLinearDimension(Document doc)
//         {
//             Application app = doc.Application;
//             //first create two lines
//             XYZ pt1 = new XYZ(5, 5, 0);
//             XYZ pt2 = new XYZ(5, 10, 0);
//             Line line = Line.CreateBound(pt1, pt2);
//
//             Plane plane = Plane.CreateByNormalAndOrigin(pt1.CrossProduct(pt2), pt2);
//             SketchPlane sketchPlane = SketchPlane.Create(doc, plane);
//             ModelCurve modelCurve1 = doc.FamilyCreate.NewModelCurve(line, sketchPlane);
//
//             pt1 = new XYZ(10, 5, 0);
//             pt2 = new XYZ(10, 10, 0);
//             line = Line.CreateBound(pt1, pt2);
//             plane = Plane.CreateByNormalAndOrigin(pt1.CrossProduct(pt2), pt2);
//
//             sketchPlane = SketchPlane.Create(doc, plane);
//
//             ModelCurve modelCurve2 = doc.FamilyCreate.NewModelCurve(line, sketchPlane);
//
//             //now create a linear dimension between them
//             ReferenceArray ra = new ReferenceArray();
//             ra.Append(modelCurve1.GeometryCurve.Reference);
//             ra.Append(modelCurve2.GeometryCurve.Reference);
//
//             pt1 = new XYZ(5, 10, 0);
//             pt2 = new XYZ(10, 10, 0);
//             line = Line.CreateBound(pt1, pt2);
//             Dimension dim = doc.FamilyCreate.NewLinearDimension(doc.ActiveView, line, ra);
//
//             //create a label for the dimension called "width"
//             FamilyParameter param = doc.FamilyManager.AddParameter("width", BuiltInParameterGroup.PG_CONSTRAINTS,
//                 ParameterType.Length, false);
//             dim.FamilyLabel = param;
//             return dim;
//         }
// #endregion
//
//         private const string _prompt = "Please selcct two parallel opppsing straght walls.";
//
// #region Create Dimension Element
//         public static void CreateDimensionElement(View view, XYZ p1, Reference r1, XYZ p2, Reference r2)
//         {
//             Document doc = view.Document;
//             ReferenceArray ra = new ReferenceArray();
//             ra.Append(r1);
//             ra.Append(r2);
//             Line line = Line.CreateBound(p1, p2);
//             using (Transaction t = new Transaction(doc))
//             {
//                 t.Start("Create new Dimension");
//                 Dimension dim = doc.Create.NewDimension(view, line, ra);
//
//                 t.Commit();
//             }
//         }
// #endregion
//
// #region GetColsetFace
//         static Face GetClosestFace(Element e, XYZ p, XYZ normal, Options opt)
//         {
//             Face face = null;
//             double min_distance = double.MaxValue;
//             GeometryElement geo = e.get_Geometry(opt);
//
//             foreach (GeometryObject obj in geo)
//             {
//                 Solid solid = obj as Solid;
//                 if (solid != null)
//                 {
//                     FaceArray fa = solid.Faces;
//                     foreach (Face f in fa)
//                     {
//                         PlanarFace pf = f as PlanarFace;
//                         Debug.Assert(null != pf, "excepted planar wall faces");
//
//                         if (null != pf && Util.IsParallel(normal, pf.FaceNormal))
//                         {
//                             XYZ v = p - pf.Origin;
//                             double d = v.DotProduct(-pf.FaceNormal);
//                             if (d < min_distance)
//                             {
//                                 face = f;
//                                 min_distance = d;
//                             }
//                         }
//                     }
//                 }
//             }
//             return face;
//         }
// #endregion
//
//
//         public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
//         {
//             UIApplication uiapp = commandData.Application;
//             UIDocument uidoc = uiapp.ActiveUIDocument;
//             Application app = uiapp.Application;
//             Document doc = uidoc.Document;
//
//             ICollection<ElementId> ids = uidoc.Selection.GetElementIds();
//             List<Wall> walls = new List<Wall>(2);
//             foreach (ElementId id in ids)
//             {
//                 Element e = doc.GetElement(id);
//                 if (e is Wall)
//                 {
//                     walls.Add(e as Wall);
//                 }
//             }
//
//             if (2 != walls.Count)
//             {
//                 message = _prompt;
//                 return Result.Failed;
//             }
//
//             // ensure the two selected walls are straight and parallel;
//             // determine their mutual[共同的] normal vector and a point on each wall for distance calculations.
//
//             List<Line> lines = new List<Line>(2);
//             List<XYZ> midPoints = new List<XYZ>(2);
//             XYZ normal = null;
//
//             foreach (Wall wall in walls)
//             {
//                 LocationCurve lc = wall.Location as LocationCurve;
//                 Curve curve = lc.Curve;
//
//                 if (!(curve is Line))
//                 {
//                     message = _prompt;
//                     return Result.Failed;
//                 }
//                 Line l = curve as Line;
//                 lines.Add(l);
//                 midPoints.Add(Util.Midpoint(l));
//
//                 if (null == normal)
//                 {
//                     normal = Util.Normal(l);
//                 }
//                 else
//                 {
//                     if (!Util.IsParallel(normal, Util.Normal(l)))
//                     {
//                         message = _prompt;
//                         return Result.Failed;
//                     }
//                 }
//             }
//
//             //find the two closet facing  faces on the walls;
//             //they are vertical faces that are parallel to the
//             //wall curve and closet to the other wall.
//
//             Options opt = app.Create.NewGeometryOptions();
//             opt.ComputeReferences = true;
//
//             List<Face> faces = new List<Face>(2);
//             faces.Add(GetClosestFace(walls[0], midPoints[1], normal, opt));
//             faces.Add(GetClosestFace(walls[1], midPoints[0], normal, opt));
//
//             //create the dimensioning;
//             CreateDimensionElement(doc.ActiveView, midPoints[0], faces[0].Reference, midPoints[1], faces[1].Reference);
//             return Result.Succeeded;
//         }
//
// #region Dimension Filled Region Alexander
//         //类的嵌套
//         [Transaction(TransactionMode.Manual)]
//         public class CreateFillledRegionDimensionsCommand : IExternalCommand
//         {
//             public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
//             {
//                 var uiapp = commandData.Application;
//                 var uidoc = uiapp.ActiveUIDocument;
//                 var doc = uidoc.Document;
//                 var view = uidoc.ActiveGraphicalView; //这是平面试图？
//                 var filledRegions = FindFilledRegions(doc, view.Id);
//                 using (var transaction = new Transaction(doc, "filled regions dimensions"))
//                 {
//                     transaction.Start();
//                     foreach (var filledRegion in filledRegions)
//                     {
//                         CreateDimensions(filledRegion, -1 * view.RightDirection);
//                         CreateDimensions(filledRegion, view.UpDirection);
//                     }
//                     transaction.Commit();
//                 }
//                 return Result.Succeeded;
//             }
//
//
//             private static void CreateDimensions(FilledRegion filledRegion, XYZ dimensionDirection)
//             {
//                 var document = filledRegion.Document;
//                 var view = (View) document.GetElement(filledRegion.OwnerViewId);
//                 var edgesDirection = dimensionDirection.CrossProduct(view.ViewDirection);
//                 var edges = FindRegionEdges(filledRegion).Where(x => IsEdgeDirectionSatisfied(x, edgesDirection))
//                     .ToList();
//
//                 if (edges.Count < 2)
//                 {
//                     return;
//                 }
//
//                 var shift = UnitUtils.ConvertToInternalUnits(-10 * view.Scale, DisplayUnitType.DUT_CUBIC_MILLIMETERS) *
//                     edgesDirection;
//
//                 var dimensionLine =
//                     Line.CreateBound(filledRegion.get_BoundingBox(view).Min + shift, dimensionDirection);
//
//                 var references = new ReferenceArray();
//
//                 foreach (var edge in edges)
//                 {
//                     references.Append(edge.Reference);
//                 }
//
//                 document.Create.NewDimension(view, dimensionLine, references);
//             }
//
//             private static bool IsEdgeDirectionSatisfied(Edge edge, XYZ edgeDirection)
//             {
//                 var edgeCurve = edge.AsCurve() as Line;
//                 if (edgeCurve == null)
//                 {
//                     return false;
//                 }
//
//                 return edgeCurve.Direction.CrossProduct(edgeDirection).IsAlmostEqualTo(XYZ.Zero);
//                 //CrossProduct :
//                 //叉积  u ^ v = |u| |v| sin(θ) ,
//                 //在3D图像学中非常有空，叉积的结果是一个向量，
//                 //向量的方向是二维空间得到法向量，
//                 //向量的模是向量a和向量b构成的平行四边形的面积
//             }
//
//             private static IEnumerable<Edge> FindRegionEdges(FilledRegion filledRegion)
//             {
//                 var view = (View) filledRegion.Document.GetElement(filledRegion.OwnerViewId);
//                 var options = new Options
//                 {
//                     View = view, ComputeReferences = true
//                 };
//
//                 return filledRegion.get_Geometry(options).OfType<Solid>().SelectMany(x => x.Edges.Cast<Edge>());
//             }
//
//             private static IEnumerable<FilledRegion> FindFilledRegions(Document document, ElementId viewId)
//             {
//                 var collector = new FilteredElementCollector(document, viewId);
//                 return collector
//                     .OfClass(typeof(FilledRegion))
//                     .Cast<FilledRegion>();
//             }
//         }
// #endregion // Dimension Filled Region Alexander
//
// #region Dimension filled Region Jorge
//         private void CreateDimensions(FilledRegion filledRegion, XYZ dimensionDirection, string typeName)
//         {
//             var document = filledRegion.Document;
//             var view = (View) document.GetElement(filledRegion.OwnerViewId);
//
//             var edgesDirection = dimensionDirection.CrossProduct(view.ViewDirection);
//
//             var edges = FindRegionEdges(filledRegion).Where(x => IsEdgeDirectionSatisfied(x, edgesDirection)).ToList();
//
//             if (edges.Count < 2)
//             {
//                 return;
//             }
//
//             var shift = UnitUtils.ConvertToInternalUnits(-10 * view.Scale, DisplayUnitType.DUT_MILLIMETERS) *
//                 edgesDirection;
//
//             var dimensionLine = Line.CreateBound(filledRegion.get_BoundingBox(view).Min + shift, dimensionDirection);
//
//             var references =new ReferenceArray();
//
//             foreach (var edge in edges)
//             {
//                 references.Append(edge.Reference);
//             }
//
//             document.Create.NewDimension(view, dimensionLine, references);
//         }
//
//         private static bool IsEdgeDirectionSatisfied(Edge edge, XYZ edgeDirection)
//         {
//             var edgeCurve = edge.AsCurve() as Line;
//             if (edgeCurve  == null )
//             {
//                 return false;
//             }
//
//             return edgeCurve.Direction.CrossProduct(edgeDirection).IsAlmostEqualTo(XYZ.Zero);
//
//         }
//
//         private static IEnumerable<Edge> FindRegionEdges(FilledRegion filledRegion)
//         {
//             var view = (View) filledRegion.Document.GetElement(filledRegion.OwnerViewId);
//
//             var options = new Options
//             {
//                 View = view, ComputeReferences = true
//             };
//
//             return filledRegion.get_Geometry(options).OfType<Solid>().SelectMany(x => x.Edges.Cast<Edge>());
//             
//         }
//
//         private static IEnumerable<FilledRegion> FindFilledRegions(Document document, ElementId viewId)
//         {
//             var collector = new FilteredElementCollector(document, viewId);
//             return collector.OfClass(typeof(FilledRegion)).Cast<FilledRegion>();
//         }
//         
// #endregion //Dimension Filled Region Alexander [?这个是作者？]
//     }
//
// #region Dimension Filled Region Jorge
//     private void CreateDimensions(filledregion)
// #endregion
// }