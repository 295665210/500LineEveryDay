// using System;
// using System.Collections.Generic;
// using System.Diagnostics;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
// using Autodesk.Revit.Attributes;
// using Autodesk.Revit.DB;
// using Autodesk.Revit.UI;
//
// namespace CodeInBuildingCoder1
// {
//     /// <summary>
//     /// For case**** [Allow/Disallow join via Revit API]
//     /// </summary>
//     [Transaction(TransactionMode.Manual)]
//     class CmdDisallowJoin : IExternalCommand
//     {
//         public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
//         {
//             Debug.Assert(false, "setting the disallow join property was not possible" +
//                                 " prior to Revit 2012. "
//                                 + "In Revit 2012, you can use the WallUtils.DisallowWallJoinAtEnd method.");
//
//         }
//     }
// }