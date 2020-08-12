using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Events;

namespace CreateViewSection
{
    /// <summary>
    /// The helper class which give some operation about point and vector.
    /// 
    /// </summary>
    public class XYZMath
    {
        //private members
        private const Double Precision = 0.0000000001; //define a precision of double data.

        //method
        /// <summary>
        /// Find the middle point of the line.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static XYZ FindMidPoint(XYZ first, XYZ second)
        {
            double x = (first.X + second.X) / 2;
            double y = (first.Y + second.Y) / 2;
            double z = (first.Z + second.Z) / 2;
            XYZ midPoint = new XYZ(x, y, z);
            return midPoint;
        }

        /// <summary>
        /// find the distance between two points.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static double FindDistance(XYZ first, XYZ second)
        {
            double x = first.X - second.X;
            double y = first.Y - second.Y;
            double z = first.Z - second.Z;
            return Math.Sqrt(x * x + y * y + z * z);
        }

        public static XYZ FindDirection(XYZ first, XYZ second)
        {
            double x = second.X - first.X;
            double y = second.Y - first.Y;
            double z = second.Z - first.Z;
            double distance = FindDistance(first, second);
            XYZ direction = new XYZ(x / distance, y / distance, z / distance);
            return direction;
        }

        public static XYZ FindRightDirection(XYZ viewDirection)
        {
            //because this example only allow beam to be horizontal,
            //the created viewSection should be vertical.
            //the same thing can also be found when user select wall or floor.
            //so only need to turn 90 degree around z axes wil get RightDirection.
            double x = -viewDirection.Y;
            double y = viewDirection.X;
            double z = viewDirection.Z;

            XYZ direction = new XYZ(x, y, z);
            return direction;
        }

        public static XYZ FindUpDirection(XYZ viewDirection)
        {
            XYZ direction = new XYZ(0, 0, 1);
            return direction;
        }

        /// <summary>
        /// find the middle point of a profile
        /// </summary>
        /// <param name="curveArray"></param>
        /// <returns></returns>
        public static XYZ FindMiddlePoint(CurveArray curveArray)
        {
            //first from a point array which include all the end points of the curves
            List<XYZ> array = new List<XYZ>();
            foreach (Curve curve in curveArray)
            {
                XYZ first = curve.GetEndPoint(0);
                XYZ second = curve.GetEndPoint(1);
                array.Add(first);
                array.Add(second);
            }

            //second find the max and min value of three coordinate
            double maxX = array[0].X;
            double minX = array[0].X;
            double maxY = array[0].Y; // the max y coordinate in the array
            double minY = array[0].Y; // the min y coordinate in the array
            double maxZ = array[0].Z; // the max z coordinate in the array
            double minZ = array[0].Z; // the min z coordinate in the array

            foreach (XYZ curve in array)
            {
                if (maxX < curve.X)
                {
                    maxX = curve.X;
                }
                if (minX > curve.X)
                {
                    minX = curve.X;
                }
                if (maxY < curve.Y)
                {
                    maxY = curve.Y;
                }
                if (minY > curve.Y)
                {
                    minY = curve.Y;
                }
                if (maxZ < curve.Z)
                {
                    maxZ = curve.Z;
                }
                if (minZ > curve.Z)
                {
                    minZ = curve.Z;
                }
            }

            //third form the middle point using the average of max and min values.
            double x = (maxX + minX) / 2;
            double y = (maxY + minY) / 2;
            double z = (maxZ + minZ) / 2;
            XYZ midPoint = new XYZ(x, y, z);
            return midPoint;
        }

        public static XYZ FindWallViewDirection(CurveArray curveArray)
        {
            XYZ direction = new XYZ();
            foreach (Curve curve in curveArray)
            {
                XYZ startPoint = curve.GetEndPoint(0);
                XYZ endPoint = curve.GetEndPoint(1);
                double distanceX = startPoint.X - endPoint.X;
                double distanceY = startPoint.Y - endPoint.Y;
                if (-Precision > distanceX || Precision < distanceX || -Precision > distanceY || Precision < distanceY)
                {
                    XYZ first = new XYZ(startPoint.X, startPoint.Y, 0);
                    XYZ second = new XYZ(endPoint.X, endPoint.Y, 0);
                    direction = FindDirection(first, second);
                    break;
                }
            }
            return direction;
        }

        public static XYZ FindFloorViewDirection(CurveArray curveArray)
        {
            Curve curve = curveArray.get_Item(0);
            XYZ first = curve.GetEndPoint(0);
            XYZ second = curve.GetEndPoint(1);
            return FindDirection(first, second);
        }
    }
}