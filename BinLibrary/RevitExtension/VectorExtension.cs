using Autodesk.Revit.DB;
using System;

namespace BinLibrary.RevitExtension
{
	public static class VectorExtension
	{
	    private static double precision = 0.000001;
        public static bool IsParallel(this XYZ vector1, XYZ vector2)
		{
			return vector1.IsSameDirection(vector2) || vector1.IsOppositeDirection(vector2);
		}
	    /// <summary>
	    /// �ж�ͬ��
	    /// </summary>
	    /// <param name="dir1"></param>
	    /// <param name="dir2"></param>
	    /// <returns></returns>
	    public static bool IsSameDirection(this XYZ dir1, XYZ dir2)
	    {
	        bool result = false;

	        double dotproduct = dir1.Normalize().DotProduct(dir2.Normalize());

	        if (Math.Abs(dotproduct - 1) < precision)
	        {
	            result = true;
	        }

	        return result;
	    }
	    /// <summary>
	    /// �жϷ���
	    /// </summary>
	    /// <param name="dir1"></param>
	    /// <param name="dir2"></param>
	    /// <returns></returns>
	    public static bool IsOppositeDirection(this XYZ dir1, XYZ dir2)
	    {
	        bool result = false;

	        double dotproduct = dir1.Normalize().DotProduct(dir2.Normalize());

	        if (Math.Abs(dotproduct + 1) < precision)
	        {
	            result = true;
	        }

	        return result;
	    }
    }
}
