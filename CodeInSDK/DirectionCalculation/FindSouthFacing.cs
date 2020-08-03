using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit;
using Autodesk.Revit.UI;


namespace DirectionCalculation
{
    /// <summary>
    /// base class for "find south facing..." utilities.
    /// </summary>
    public class FindSouthFacingBase
    {
        private Autodesk.Revit.ApplicationServices.Application m_app;
        private Document m_doc;
        private System.IO.TextWriter m_writer;


#region Helper properties
        protected Autodesk.Revit.ApplicationServices.Application Application
        {
            get { return m_app; }
            set { m_app = value; }
        }

        protected Document Document
        {
            get { return m_doc; }
            set { m_doc = value; }
        }
#endregion

        /// <summary>
        ///Identifies if a particular direction is "south facing". This means within a range of -45 degree to 45 degree to the south vector(th negative Y axis).
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected bool IsSouthFacing(XYZ direction)
        {
            double angleToSouth = direction.AngleTo(-XYZ.BasisY);
            return Math.Abs(angleToSouth) < Math.PI / 4;
        }

        /// <summary>
        /// Transform a direction vector by the rotation angle of the activeProjectLocation
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected XYZ TransformByProjectLocation(XYZ direction)
        {
            ProjectPosition position = Document.ActiveProjectLocation.GetProjectPosition(XYZ.Zero);

            Transform transform = Transform.CreateRotation(XYZ.BasisZ, position.Angle);
            XYZ rotateDirection = transform.OfVector(direction);
            return rotateDirection;
        }

#region Debugging Aids
        protected void Write(string label, Curve curve)
        {
            if (m_writer == null)
            {
                m_writer = new StreamWriter(@"c:\Directions.txt");
            }
            XYZ start = curve.GetEndPoint(0);
            XYZ end = curve.GetEndPoint(1);
            m_writer.WriteLine(string.Format(label + "{0}{1}", XYZToString(start), XYZToString(end)));
        }

        private String XYZToString(XYZ point)
        {
            return "(" + point.X + "," + point.Y + "," + point.Z + ")";
        }

        protected void CloseFile()
        {
            if (m_writer != null)
            {
                m_writer.Close();
            }
        }
#endregion
    }
}