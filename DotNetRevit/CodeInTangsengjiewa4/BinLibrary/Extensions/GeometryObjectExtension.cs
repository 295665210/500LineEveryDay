using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Autodesk.Revit.DB;

namespace CodeInTangsengjiewa4.BinLibrary.Extensions
{
    public static class GeometryObjectExtension
    {
        public static IList<Face> GetFacesOfGeometryObject(this GeometryObject geoObj)
        {
            IList<Face> result = new List<Face>();
            List<Face> temResult = new List<Face>();
            if (geoObj is GeometryElement)
            {
                GeometryElement geoele = geoObj as GeometryElement;
                foreach (GeometryObject geometryObject in geoele)
                {
                    temResult.AddRange(GetFacesOfGeometryObject(geometryObject));
                }
            }
            else if (geoObj is GeometryInstance)
            {
                GeometryElement geoele = (geoObj as GeometryInstance).SymbolGeometry;
                foreach (GeometryObject obj in geoele)
                {
                    temResult.AddRange(GetFacesOfGeometryObject(obj));
                }
            }
            else if (geoObj is Solid)
            {
                Solid solid = geoObj as Solid;
                foreach (Face face in solid.Faces)
                {
                    temResult.Add(face);
                }
            }
            result = temResult;
            return result;
        }

        public static IList<Solid> GetSolidOfGeometryObject(this GeometryObject geoObj)
        {
            IList<Solid> result = new List<Solid>();
            List<Solid> temResult = new List<Solid>();
            if (geoObj is GeometryElement)
            {
                GeometryElement geoele = geoObj as GeometryElement;
                foreach (GeometryObject geometryObject in geoele)
                {
                    temResult.AddRange(GetSolidOfGeometryObject(geometryObject));
                }
            }
            else if (geoObj is GeometryInstance)
            {
                GeometryElement geoele = (geoObj as GeometryInstance).SymbolGeometry;
                foreach (GeometryObject obj in geoele)
                {
                    if (obj is Solid)
                    {
                        temResult.AddRange(GetSolidOfGeometryObject(obj));
                    }
                }
            }
            else if (geoObj is Solid)
            {
                Solid solid = geoObj as Solid;
                temResult.Add(solid);
            }
            result = temResult;
            return result;
        }

        public static IList<Edge> GetEdgesOfGeometryObject(this GeometryObject geoobj)
        {
            IList<Edge> result = new List<Edge>();
            List<Edge> temResult = new List<Edge>();
            if (geoobj is GeometryElement)
            {
                GeometryElement geoele = geoobj as GeometryElement;
                foreach (GeometryObject geoItem in geoele)
                {
                    temResult.AddRange(GetEdgesOfGeometryObject(geoItem));
                }
            }

            else if (geoobj is GeometryInstance)
            {
                GeometryElement geoele = (geoobj as GeometryInstance).SymbolGeometry;
                foreach (GeometryObject obj in geoele)
                {
                    if (obj is Solid)
                    {
                        temResult.AddRange(GetEdgesOfGeometryObject(obj));
                    }
                }
            }

            else if (geoobj is Solid)
            {
                Solid solid = geoobj as Solid;
                foreach (Face face in solid.Faces)
                {
                    temResult.AddRange(GetEdgesOfGeometryObject(face));
                }
            }

            else if (geoobj is Face)
            {
                Face face = geoobj as Face;
                foreach (EdgeArray edgeArray in face.EdgeLoops)
                {
                    var enu = edgeArray.GetEnumerator();
                    while (enu.MoveNext())
                    {
                        var edge = enu.Current as Edge;
                        if (edge != null)
                        {
                            temResult.Add(edge);
                        }
                    }
                }
            }
            else if (geoobj is Edge)
            {
                temResult.Add(geoobj as Edge);
            }
            result = temResult;
            return result;
        }
    }
}