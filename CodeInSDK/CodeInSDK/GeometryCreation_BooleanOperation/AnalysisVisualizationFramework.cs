using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;

namespace CodeInSDK.GeometryCreation_BooleanOperation.CS
{
    class AnalysisVisualizationFramework
    {
        /// <summary>
        /// the singleton instance of AnalysisVisualizationFramework
        /// </summary>
        private static AnalysisVisualizationFramework Instance;

        /// <summary>
        /// revit document
        /// </summary>
        private Autodesk.Revit.DB.Document m_doc;

        /// <summary>
        /// The created view list
        /// </summary>
        private List<String> viewNameList = new List<string>();

        /// <summary>
        /// The ID of schema which SpatialFieldManager register
        /// </summary>
        private static int SchemaId = -1;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="doc"></param>
        private AnalysisVisualizationFramework(Autodesk.Revit.DB.Document doc)
        {
            m_doc = doc;
        }

        /// <summary>
        /// Get the singleton instance of AnalysisVisualizationFramework
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static AnalysisVisualizationFramework getInstance(Autodesk.Revit.DB.Document doc)
        {
            if (Instance == null)
            {
                Instance = new AnalysisVisualizationFramework(doc);
            }
            return Instance;
        }

        public void PaintSolid(Solid s, String viewName)
        {
            View view;
            if (!viewNameList.Contains(viewName))
            {
                IList<Element> viewFamilyTypes =
                    new FilteredElementCollector(m_doc).OfClass(typeof(ViewFamilyType)).ToElements();
                ElementId view3DId = new ElementId(-1);
                foreach (Element e in viewFamilyTypes)
                {
                    if (e.Name == "三维视图")
                    {
                        view3DId = e.Id;
                    }
                }
                view = View3D.CreateIsometric(m_doc, view3DId);
                ViewOrientation3D viewOrientation3D =
                    new ViewOrientation3D(new XYZ(1, -1, -1), new XYZ(1, 1, 1), new XYZ(1, 1, -2));
                (view as View3D).SetOrientation(viewOrientation3D);
                (view as View3D).SaveOrientation();
                view.Name = viewName;
                viewNameList.Add(viewName);
            }
            else
            {
                view = (
                    (new FilteredElementCollector(m_doc)
                        .OfClass(typeof(View))
                        .Cast<View>())
                    .Where(e => e.Name == viewName)
                    )
                    .First<View>();
            }

            SpatialFieldManager sfm = SpatialFieldManager.GetSpatialFieldManager(view);
            if (sfm == null)
            {
                sfm = SpatialFieldManager.CreateSpatialFieldManager(view, 1);
            }

            if (SchemaId != -1)
            {
                IList<int> results = sfm.GetRegisteredResults();
                if (!results.Contains(SchemaId))
                {
                    SchemaId = -1;
                }
            }

            if (SchemaId == -1)
            {
                AnalysisResultSchema resultSchema1 = new AnalysisResultSchema("PaintedSolid" + viewName, "Description");

                AnalysisDisplayStyle displayStyle = AnalysisDisplayStyle.CreateAnalysisDisplayStyle(
                    m_doc,
                    "Real_Color_Surface" + view.Name,
                    new AnalysisDisplayColoredSurfaceSettings(),
                    new AnalysisDisplayColorSettings(),
                    new AnalysisDisplayLegendSettings());
                resultSchema1.AnalysisDisplayStyleId = displayStyle.Id;
                SchemaId = sfm.RegisterResult(resultSchema1);
            }

            FaceArray faces = s.Faces;
            Transform trf = Transform.Identity;

            foreach (Face face in faces)
            {
                int idx = sfm.AddSpatialFieldPrimitive(face, trf);

                IList<UV> uvPts = null;
                IList<ValueAtPoint> valList = null;
                ComputeValueAtPointForFace(face, out uvPts, out valList, 1);

                FieldDomainPointsByUV pnts = new FieldDomainPointsByUV(uvPts);

                FieldValues vals = new FieldValues(valList);

                sfm.UpdateSpatialFieldPrimitive(idx, pnts, vals, SchemaId);
            }
        }

        private static void ComputeValueAtPointForFace
            (Face face, out IList<UV> uvPts, out IList<ValueAtPoint> valList, int measurementNo)
        {
            List<double> doubleList = new List<double>();
            uvPts = new List<UV>();
            valList = new List<ValueAtPoint>();

            BoundingBoxUV bb = face.GetBoundingBox();

            for (double u = bb.Min.U; u < bb.Max.U + 0.0000001; u = u + (bb.Max.U - bb.Min.U) / 1)

            {
                for (double v = bb.Min.V; v < bb.Max.V + 0.0000001; v = v + (bb.Max.V - bb.Min.V) / 1)
                {
                    UV uvPnt = new UV(u, v);
                    uvPts.Add(uvPnt);
                    XYZ faceXYZ = face.Evaluate(uvPnt);

                    // Specify three values for each point
                    for (int ii = 1; ii <= measurementNo; ii++)
                    {
                        doubleList.Add(faceXYZ.DistanceTo(XYZ.Zero) * ii);
                    }
                    valList.Add(new ValueAtPoint(doubleList));
                    doubleList.Clear();
                }
            }
        }
    }
}