// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
// using Autodesk.Revit.Attributes;
// using Autodesk.Revit.DB;
// using Autodesk.Revit.UI;
// using Autodesk.Revit.UI.Selection;
// using CodeInTangsengjiewa.BinLibrary.Extensions;
//
// namespace CodeInTangsengjiewa4.CodeOfQian
// {
//     /// <summary>
//     /// dim twp point
//     /// </summary>
//     [Transaction(TransactionMode.Manual)]
//     [Regeneration(RegenerationOption.Manual)]
//     [Journaling(JournalingMode.UsingCommandData)]
//    public class Cmd_DimTwoPoint:IExternalCommand
//     {
//         public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
//         {
//             var uiapp = commandData.Application;
//             var app = uiapp.Application;
//             var uidoc = uiapp.ActiveUIDocument;
//             var doc = uidoc.Document;
//             var sel = uidoc.Selection;
//             var view = doc.ActiveView;
//             ViewType viewType = view.ViewType;
//
//             if (viewType == ViewType.FloorPlan ||viewType == ViewType.Elevation)
//             {
//                 Reference eRef = sel.PickObject(ObjectType.Element, "please select a curve based element like wall");
//                 Element element = eRef.GetElement(doc);
//                 if (eRef != null && element != null)
//                 {
//                     
//                 }
//             }
//         }
//     }
// }