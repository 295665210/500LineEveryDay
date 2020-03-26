// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using System.Windows.Documents;
// using System.Text;
// using System.Threading.Tasks;
// using System.Windows.Documents;
// using Autodesk.Revit.ApplicationServices;
// using Autodesk.Revit.Attributes;
// using Autodesk.Revit.DB;
// using Autodesk.Revit.DB.Events;
// using Autodesk.Revit.UI;
//     
// using CodeInTangsengjiewa3.BinLibrary.Extensions;
// using CodeInTangsengjiewa3.BinLibrary.RevitHelper;
// using CodeInTangsengjiewa3.注释.UIs;
//
// namespace CodeInTangsengjiewa3.样板
// {
//     /// <summary>
//     ///  统计长度
//     /// </summary>
//     [Transaction(TransactionMode.Manual)]
//     [Regeneration(RegenerationOption.Manual)]
//     [Journaling(JournalingMode.UsingCommandData)]
//     public class Cmd_LengthAccumulate : IExternalCommand
//     {
//         public static List<ModelLine> ModelLines = new List<ModelLine>();
//         public static List<ElementId> addedIds = new List<ElementId>();
//         public static Document _doc = default(Document);
//
//         public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
//         {
//             var uiapp = commandData.Application;
//             var uidoc = uiapp.ActiveUIDocument;
//             var doc = uidoc.Document;
//             var acview = doc.ActiveView;
//             var sel = uidoc.Selection;
//
//             _doc = doc;
//
//             doc.Application.DocumentChanged += OnDocumentChanged;
//
//             ResultShow resultShowin = ResultShow.Instance;
//             resultShowin.Helper().Owner = RevitWindowHelper.GetRevitHandle();
//             ResultShow.Instance.Show();
//             uiapp.PostCommand(RevitCommandId.LookupPostableCommandId(PostableCommand.ModelLine));
//             return Result.Succeeded;
//         }
//
//         private void OnDocumentChanged(object sender, DocumentChangedEventArgs e)
//         {
//             var app = sender as Application;
//             var ids = e.GetAddedElementIds();
//             foreach (var elementId in ids)
//             {
//                 if (!addedIds.Contains(elementId))
//                 {
//                     addedIds.Add(elementId);
//                 }
//             }
//         }
//     }
// }