﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace CodeInSDK.GeometryCreation_BooleanOperation
{
    static class BooleanOperation
    {
        /// <summary>
        /// Boolean intersect geometric operation,return a new solid as the result
        /// </summary>
        /// <param name="solid1"></param>
        /// <param name="solid2"></param>
        /// <returns></returns>
        public static Solid BooleanOperation_Intersect(Solid solid1, Solid solid2)
        {
            return BooleanOperationsUtils.ExecuteBooleanOperation(solid1, solid2, BooleanOperationsType.Intersect);
        }

        /// <summary>
        /// Boolean union geometric operation, return a new solid as the result 
        /// </summary>
        /// <param name="solid1"></param>
        /// <param name="solid2"></param>
        /// <returns></returns>
        public static Solid BooleanOperation_Union(Solid solid1, Solid solid2)
        {
            return BooleanOperationsUtils.ExecuteBooleanOperation(solid1, solid2, BooleanOperationsType.Union);
        }

        /// <summary>
        ///  Boolean difference geometric operation, return a new solid as the result
        /// </summary>
        /// <param name="solid1"></param>
        /// <param name="solid2"></param>
        /// <returns></returns>
        public static Solid BooleanOperation_Difference(Solid solid1, Solid solid2)
        {
            return BooleanOperationsUtils.ExecuteBooleanOperation(solid1, solid2, BooleanOperationsType.Difference);
        }

        /// <summary>
        /// Boolean intersect geometric operation, modify the original solid as result.
        /// </summary>
        /// <param name="solid1"></param>
        /// <param name="solid2"></param>
        public static void BooleanOperation_Intersect(ref Solid solid1, Solid solid2)
        {
            BooleanOperationsUtils.ExecuteBooleanOperationModifyingOriginalSolid(solid1, solid2,
                BooleanOperationsType.Intersect);
        }

        /// <summary>
        /// Boolean union geometric operation, modify the original solid as the result
        /// </summary>
        /// <param name="solid1"></param>
        /// <param name="solid2"></param>
        public static void BooleanOperation_Union(ref Solid solid1, Solid solid2)
        {
            BooleanOperationsUtils.ExecuteBooleanOperationModifyingOriginalSolid(solid1, solid2,
                BooleanOperationsType.Union);
        }

        /// <summary>
        /// Boolean difference geometric operation , modify the original solid as the result
        /// </summary>
        /// <param name="solid1"></param>
        /// <param name="solid2"></param>
        public static void BooleanOperation_Difference(ref Solid solid1, Solid solid2)
        {
            BooleanOperationsUtils.ExecuteBooleanOperationModifyingOriginalSolid(solid1, solid2,
                BooleanOperationsType.Difference);
        }
    }
}