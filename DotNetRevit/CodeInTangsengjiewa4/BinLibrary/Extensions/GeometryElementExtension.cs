using Autodesk.Revit.DB;
using System.Collections.Generic;


namespace CodeInTangsengjiewa4.BinLibrary.Extensions
{
    public static class GeometryElementExtension
    {
        public static List<GeometryObject> GetGeometries(this GeometryElement geoele)
        {
            List<GeometryObject> result = new List<GeometryObject>();
            var enu = geoele.GetEnumerator();
            while (enu.MoveNext())
            {
                var geoobj = enu.Current as GeometryObject;
                if (geoobj != null)
                {
                    result.Add(geoobj);
                }
            }
            return result;
        }

        public static List<Face> GetFaces(this GeometryElement geoele)
        {
            List<Face> result = new List<Face>();
            var geoobjs = geoele.GetGeometries();
            foreach (GeometryObject geometryObject in geoobjs)
            {
                result.AddRange(geometryObject.GetFacesOfGeometryObject());
            }
            return result;
        }

        public static List<Edge> GetEdges(this GeometryElement geoele)
        {
            List<Edge> result = new List<Edge>();
            var geoobjs = geoele.GetGeometries();
            foreach (GeometryObject geoobj in geoobjs)
            {
                result.AddRange(geoobj.GetEdgesOfGeometryObject());
            }
            return result;
        }

        public static List<XYZ> GetPoints(this GeometryElement geoele)
        {
            List<XYZ> result = new List<XYZ>();
            var geoedges = geoele.GetEdges();
            var points = new List<XYZ>();

            foreach (var edge in geoedges)
            {
                var curve = edge.AsCurve();
                var startPoint = curve.GetEndPoint(0);
                var endPoint = curve.GetEndPoint(1);
                //判断点事否位置上重合,如果不重合,则添加进列表
                var startFlag = false;
                var endFlag = false;
                points.ForEach(m =>
                {
                    if (m.DistanceTo(startPoint) < 1e-6)
                    {
                        startFlag = true;
                    }
                    if (m.DistanceTo(endPoint) < 1e-6)
                    {
                        endFlag = true;
                    }
                });

                if (!startFlag)
                {
                    points.Add(startPoint);
                }
                if (!endFlag)
                {
                    points.Add(endPoint);
                }
            }
            result = points;
            return result;
        }
    }
}