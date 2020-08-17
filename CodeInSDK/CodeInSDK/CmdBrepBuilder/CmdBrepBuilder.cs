// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
// using Autodesk.Revit.DB;
// using Autodesk.Revit.UI;
//

//?????????看不懂在干什么。
// namespace CodeInSDK.CmdBrepBuilder
// {
//     class CmdBrepBuilder : IExternalCommand
//     {
//         public BRepBuilder CreateBrepSolid()
//         {
//             BRepBuilder b = new BRepBuilder(BRepType.Solid);
//             //1 Planes
//             //naming convention for faces and planes:
//             //we are looking at this cube in an isometric view.
//             //X is down and to the left of us.Y is horizontal
//             //and points to the right , Z is up.
//             //front and back faces are along the X axis, left and right
//             //are along the Y axis, top and bottom are along the Z axis.
//
//             Plane bottom = Plane.CreateByOriginAndBasis(new XYZ(50, 50, 0), new XYZ(1, 0, 0), new XYZ(0, 1, 0));
//             //bottom .XY plane , z =0 , normal pointing inside the cube.
//             Plane top = Plane.CreateByOriginAndBasis(new XYZ(50, 50, 100), new XYZ(1, 0, 0), new XYZ(0, 1, 0));
//             //top .XY plane, Z = 100 ,normal pointing outside the cube.
//             Plane front = Plane.CreateByOriginAndBasis(new XYZ(100, 50, 50), new XYZ(0, 0, 1), new XYZ(0, 1, 0));
//             //front side .ZY plane, x= 0 ,normal pointing inside the cube.
//             Plane back = Plane.CreateByOriginAndBasis(new XYZ(0, 50, 50), new XYZ(0, 0, 1), new XYZ(0, 1, 0));
//             //back side .ZY plane, x =0, normal pointing outside the cube.
//             Plane left = Plane.CreateByOriginAndBasis(new XYZ(50, 0, 50), new XYZ(0, 0, 1), new XYZ(1, 0, 0));
//             //left side .ZX plane, Y = 0 , normal pointing inside the cube.
//             Plane right = Plane.CreateByOriginAndBasis(new XYZ(50, 100, 50), new XYZ(0, 0, 1), new XYZ(1, 0, 0));
//             //right side .ZX plane, Y = 100 , normal pointing outside the cube.
//
//             //2 Faces.
//             BRepBuilderGeometryId faceId_Bottom = 
//                 b.AddFace(BRepBuilderSurfaceGeometry.Create(bottom, null), true);
//             BRepBuilderGeometryId faceId_Top =
//                 b.AddFace(BRepBuilderSurfaceGeometry.Create(top, null), true);
//             BRepBuilderGeometryId faceId_Front =
//                 b.AddFace(BRepBuilderSurfaceGeometry.Create(front, null), true);
//             BRepBuilderGeometryId faceId_Back = 
//                 b.AddFace(BRepBuilderSurfaceGeometry.Create(back, null), true);
//             BRepBuilderGeometryId faceId_Left = 
//                 b.AddFace(BRepBuilderSurfaceGeometry.Create(left, null), true);
//             BRepBuilderGeometryId faceId_Right =
//                 b.AddFace(BRepBuilderSurfaceGeometry.Create(right, null), true);
//
//             //3. Edges.
//             //3.a define edge geometry
//             //walk around bottom face
//
//         }
//
//         public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
//         {
//             throw new NotImplementedException();
//         }
//     }
// }