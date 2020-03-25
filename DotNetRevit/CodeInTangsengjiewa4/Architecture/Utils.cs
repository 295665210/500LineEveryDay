using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using CodeInTangsengjiewa4.BinLibrary.Extensions;

namespace CodeInTangsengjiewa4.Architecture
{
    public class Utils
    {
        public static bool CutBeam(FamilyInstance beam, XYZ cutPoint)
        {
            var result = false;
            if (beam.Category.Id.IntegerValue != (int) BuiltInCategory.OST_StructuralFraming)
            {
                throw new Exception("element being cut is not beam");
            }
            var locationLine = beam.Location as LocationCurve;
            ///未写完
            ///
            return result;
        }
    }

    public static class TemUtils
    {
        public static List<Face> GetUpFaces(this Solid solid)
        {
            var upFaces = new List<Face>();
            var faces = solid.Faces;
            foreach (Face face in faces)
            {
                var normal = face.ComputeNormal(new UV());
                if (normal.IsSameDirection(XYZ.BasisZ))
                {
                    upFaces.Add(face);
                }
            }
            return upFaces;
        }

        public static List<Face> GetUpFaces(this GeometryObject geoObj)
        {
            var solids = geoObj.GetSolids();
            var upFaces = new List<Face>();
            foreach (var solid in solids)
            {
                var temUpFaces = solid.GetUpFaces();
                if (temUpFaces.Count > 0)
                {
                    upFaces.AddRange(temUpFaces);
                }
            }
            return upFaces;
        }

        public static List<Solid> GetSolids(this GeometryObject geoObj)
        {
            var solids = new List<Solid>();
            if (geoObj is Solid solid)
            {
                solids.Add(solid);
            }
            else if (geoObj is GeometryInstance geoInstance)
            {
                var geometryElement = geoInstance.SymbolGeometry;
                var enu = geometryElement.GetEnumerator();
                while (enu.MoveNext())
                {
                    var temGeoobj = enu.Current as GeometryObject;
                    solids.AddRange(GetSolids(geoObj));
                }
            }
            else if (geoObj is GeometryElement geoElement)
            {
                var enu = geoElement.GetEnumerator();
                while (enu.MoveNext())
                {
                    var temGeoobj = enu.Current as GeometryObject;
                    solids.AddRange(GetSolids(temGeoobj));
                }
            }
            return solids;
        }

        public static List<Solid> GetSolids(this GeometryObject geoObj, Transform trs)
        {
            var solids = new List<Solid>();
            if (geoObj is Solid solid)
            {
                if (trs != null || trs != Transform.Identity)
                {
                    solid = SolidUtils.CreateTransformed(solid, trs);
                }
                solids.Add(solid);
            }
            else if (geoObj is GeometryInstance geoInstance)
            {
                var transform = geoInstance.Transform;
                var symbolGeometry = geoInstance.SymbolGeometry;
                var enu = symbolGeometry.GetEnumerator();
                while (enu.MoveNext())
                {
                    var temGeoobj = enu.Current as GeometryObject;
                    solids.AddRange(GetSolids(temGeoobj, transform));
                }
            }
            else if (geoObj is GeometryElement geoElement)
            {
                var enu = geoElement.GetEnumerator();
                while (enu.MoveNext())
                {
                    var temGeoObj = enu.Current as GeometryObject;
                    solids.AddRange(GetSolids(temGeoObj, trs));
                }
            }
            return solids;
        }

        public static List<GeometryObject> GetSolids(this Element element)
        {
            var result = new List<GeometryObject>();
            var geometryEle = element.get_Geometry(new Options()
            {
                DetailLevel = ViewDetailLevel.Fine
            });
            var enu = geometryEle.GetEnumerator();
            while (enu.MoveNext())
            {
                var curGeoobj = enu.Current;
                if (curGeoobj is Solid solid)
                {
                    result.Add(solid);
                }
                else if (curGeoobj is GeometryInstance)
                {
                    result.AddRange(GetSolids(curGeoobj));
                }
                else if (curGeoobj is GeometryElement)
                {
                    result.AddRange(GetSolids(curGeoobj));
                }
            }
            return result;
        }

        public static CurveArray ToCurveArray(this CurveLoop curveLoop)
        {
            var result = new CurveArray();
            foreach (Curve curve in curveLoop)
            {
                result.Append(curve);
            }
            return result;
        }

        public static Solid MergeSolids(Solid solid1, Solid solid2)
        {
            var result = default(Solid);
            try
            {
                result = BooleanOperationsUtils.ExecuteBooleanOperation(solid1, solid2, BooleanOperationsType.Union);
            }
            catch
            {
                result = null;
            }
            return result;
        }

        public static Solid MergeSolids(List<Solid> solids)
        {
            var result = default(Solid);
            foreach (var solid in solids)
            {
                if (result == null)
                {
                    result = solid;
                }
                else
                {
                    var temSolid = MergeSolids(result, solid);
                    if (temSolid == null)
                    {
                        continue;
                    }
                    result = temSolid;
                }
            }
            return result;
        }
    }
}