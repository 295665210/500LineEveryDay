using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInTangsengjiewa4.BinLibrary.Extensions
{
    public static class DoubleExtension
    {
        public static double precision = 1e-6;

        public static bool IsEqual(this double num1, double num2)
        {
            double subtract = num1 - num2;
            if (Math.Abs(subtract) < precision)
            {
                return true;
            }
            return false;
        }

        public static double MetricToFeet(this double double1)
        {
            return double1 / 304.8;
        }

        public static double MmToFeet(this double double1)
        {
            return double1 / 304.8;
        }

        public static double FeetToMetric(this double double1)
        {
            return double1 * 304.8;
        }

        public static double FeetToMm(this double double1)
        {
            return double1 * 304.8;
        }

        public static double RadiusToDegree(this double double1)
        {
            return double1 * 180 / Math.PI;
        }

        public static double DegreeToRadius(this double double1)
        {
            return double1 * Math.PI / 180;
        }
    }
}