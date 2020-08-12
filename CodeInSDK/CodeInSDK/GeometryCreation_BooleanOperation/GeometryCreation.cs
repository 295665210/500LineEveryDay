using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace CodeInSDK.GeometryCreation_BooleanOperation
{
    class GeometryCreation
    {
        /// <summary>
        /// The singleton instance of GeometryCreation
        /// </summary>
        private static GeometryCreation Instance;
        /// <summary>
        /// revit application
        /// </summary>
        private Autodesk.Revit.ApplicationServices.Application m_app;

        public enum CylinderDirection
        {
            BasisX,
            BasisY,
            BasisZ
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="app"></param>
        private GeometryCreation(Autodesk.Revit.ApplicationServices.Application app)
        {
            m_app = app;
        }

        /// <summary>
        /// Get the singleton instance of GeometryCreation
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static GeometryCreation getInstance(Autodesk.Revit.ApplicationServices.Application app)
        {
            if (Instance == null)
            {
                Instance = new GeometryCreation(app);
            }
            return Instance;
        }

        /// <summary>
        /// Create an extrusion geometry
        /// </summary>
        /// <param name="profileLoops"></param>
        /// <param name="extrusionDir"></param>
        /// <param name="extrusionDist"></param>
        /// <returns></returns>
        private Solid CreateExtrusion(List<CurveLoop> profileLoops, XYZ extrusionDir, double extrusionDist)
        {
            return GeometryCreationUtilities.CreateExtrusionGeometry(profileLoops, extrusionDir, extrusionDist);
        }

        /// <summary>
        /// Create a swept geometry
        /// </summary>
        /// <param name="coordinateFrame"></param>
        /// <param name="profileLoops"></param>
        /// <param name="starAngle"></param>
        /// <param name="endAngle"></param>
        /// <returns></returns>
        private Solid CreateRevolved
            (Autodesk.Revit.DB.Frame coordinateFrame, List<CurveLoop> profileLoops, double starAngle, double endAngle)
        {
            return GeometryCreationUtilities.CreateRevolvedGeometry(coordinateFrame, profileLoops, starAngle, endAngle);
        }

        /// <summary>
        /// Create a swept geometry
        /// </summary>
        /// <param name="sweepPath"></param>
        /// <param name="pathAttachmentCrvIdx"></param>
        /// <param name="pathAttachmentParam"></param>
        /// <param name="profileLoops"></param>
        private Solid CreateSwept
            (CurveLoop sweepPath, int pathAttachmentCrvIdx, double pathAttachmentParam, List<CurveLoop> profileLoops)
        {
            return GeometryCreationUtilities.CreateSweptGeometry(sweepPath, pathAttachmentCrvIdx, pathAttachmentParam,
                profileLoops);
        }

        /// <summary>
        /// Create a blend geometry
        /// </summary>
        /// <param name="firstLoop"></param>
        /// <param name="secondLoop"></param>
        /// <param name="vertexPairs"></param>
        /// <returns></returns>
        private Solid CreateBlend(CurveLoop firstLoop, CurveLoop secondLoop, List<VertexPair> vertexPairs)
        {
            return GeometryCreationUtilities.CreateBlendGeometry(firstLoop, secondLoop, vertexPairs);
        }

        /// <summary>
        /// Create a swept and blend geometry
        /// </summary>
        /// <param name="pathCurve"></param>
        /// <param name="pathParams"></param>
        /// <param name="profileLoops"></param>
        /// <param name="vertexPairs"></param>
        /// <returns></returns>
        private Solid CreateSweptBlend
        (
            Curve pathCurve, List<double> pathParams, List<CurveLoop> profileLoops,
            List<ICollection<VertexPair>> vertexPairs
        )
        {
            return GeometryCreationUtilities.CreateSweptBlendGeometry(pathCurve, pathParams, profileLoops, vertexPairs);
        }

        /// <summary>
        /// Create a center based box
        /// </summary>
        /// <param name="center"></param>
        /// <param name="edgelength"></param>
        /// <returns></returns>
        public Solid CreateCenterbasedBox(XYZ center, double edgelength)
        {
            double halfedgelength = edgelength / 2.0;

            List<CurveLoop> profileloops = new List<CurveLoop>();
            CurveLoop profileloop = new CurveLoop();
            profileloop.Append(Line.CreateBound(
                new XYZ(center.X - halfedgelength, center.Y - halfedgelength, center.Z - halfedgelength),
                new XYZ(center.X - halfedgelength, center.Y + halfedgelength, center.Z - halfedgelength)));
            profileloop.Append(Line.CreateBound(
                new XYZ(center.X - halfedgelength, center.Y + halfedgelength, center.Z - halfedgelength),
                new XYZ(center.X + halfedgelength, center.Y + halfedgelength, center.Z - halfedgelength)));
            profileloop.Append(Line.CreateBound(
                new XYZ(center.X + halfedgelength, center.Y + halfedgelength, center.Z - halfedgelength),
                new XYZ(center.X + halfedgelength, center.Y - halfedgelength, center.Z - halfedgelength)));
            profileloop.Append(Line.CreateBound(
                new XYZ(center.X + halfedgelength, center.Y - halfedgelength, center.Z - halfedgelength),
                new XYZ(center.X - halfedgelength, center.Y - halfedgelength, center.Z - halfedgelength)));
            profileloops.Add(profileloop);

            XYZ extrusiondir = new XYZ(0, 0, 1);

            double extrusiondist = edgelength;

            return GeometryCreationUtilities.CreateExtrusionGeometry(profileloops, extrusiondir, extrusiondist);
        }

        /// <summary>
        /// create a center based sphere
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public Solid CreateCenterbasedSphere(XYZ center, double radius)
        {
            Frame frame = new Frame(center, XYZ.BasisX, XYZ.BasisY, XYZ.BasisZ);

            List<CurveLoop> profileloops = new List<CurveLoop>();
            CurveLoop profileLoop = new CurveLoop();
            Curve cemiEllipse = Ellipse.CreateCurve(center, radius, radius, XYZ.BasisX, XYZ.BasisZ, -Math.PI / 2.0,
                Math.PI / 2.0);
            profileLoop.Append(cemiEllipse);
            profileLoop.Append(Line.CreateBound(
                new XYZ(center.X, center.Y, center.Z + radius),
                new XYZ(center.X, center.Y, center.Z - radius)));
            profileloops.Add(profileLoop);

            return GeometryCreationUtilities.CreateRevolvedGeometry(frame, profileloops, -Math.PI, Math.PI);
        }

        /// <summary>
        /// [Cylinder 圆柱】
        /// </summary>
        /// <returns></returns>
        public Solid CreateCenterbasedCylinder
            (XYZ center, double bottomradius, double height, CylinderDirection cylinderdirection)
        {
            double halfheight = height / 2.0;

            XYZ bottomcenter = new XYZ(
                cylinderdirection == CylinderDirection.BasisX ? center.X - halfheight : center.X,
                cylinderdirection == CylinderDirection.BasisY ? center.Y - halfheight : center.Y,
                cylinderdirection == CylinderDirection.BasisZ ? center.Z - halfheight : center.Z);

            XYZ topcenter = new XYZ(
                cylinderdirection == CylinderDirection.BasisX ? center.X + halfheight : center.X,
                cylinderdirection == CylinderDirection.BasisY ? center.Y + halfheight : center.Y,
                cylinderdirection == CylinderDirection.BasisZ ? center.Z + halfheight : center.Z);

            CurveLoop sweepPath = new CurveLoop();
            sweepPath.Append(Line.CreateBound(bottomcenter, topcenter));

            List<CurveLoop> profileloops = new List<CurveLoop>();
            CurveLoop profileloop = new CurveLoop();

            Curve cemiEllipse1 = Ellipse.CreateCurve(bottomcenter, bottomradius, bottomradius,
                cylinderdirection == CylinderDirection.BasisX
                    ? Autodesk.Revit.DB.XYZ.BasisY
                    : Autodesk.Revit.DB.XYZ.BasisX,
                cylinderdirection == CylinderDirection.BasisZ
                    ? Autodesk.Revit.DB.XYZ.BasisY
                    : Autodesk.Revit.DB.XYZ.BasisZ,
                -Math.PI, 0);

            Curve cemiEllipse2 = Ellipse.CreateCurve(bottomcenter, bottomradius, bottomradius,
                cylinderdirection == CylinderDirection.BasisX
                    ? Autodesk.Revit.DB.XYZ.BasisY
                    : Autodesk.Revit.DB.XYZ.BasisX,
                cylinderdirection == CylinderDirection.BasisZ
                    ? Autodesk.Revit.DB.XYZ.BasisY
                    : Autodesk.Revit.DB.XYZ.BasisZ,
                0, Math.PI);

            profileloop.Append(cemiEllipse1);
            profileloop.Append(cemiEllipse2);
            profileloops.Add(profileloop);

            return GeometryCreationUtilities.CreateSweptGeometry(sweepPath, 0, 0, profileloops);
        }
    }
}