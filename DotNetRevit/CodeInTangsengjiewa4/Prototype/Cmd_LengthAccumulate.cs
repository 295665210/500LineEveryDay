using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using CodeInTangsengjiewa.BinLibrary.Extensions;
using CodeInTangsengjiewa4.BinLibrary.RevitHelper;
using CodeInTangsengjiewa4.Prototype.UIs;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace CodeInTangsengjiewa4.Prototype
{
    /// <summary>
    /// 统计长度
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class Cmd_LengthAccumulate : IExternalCommand
    {
        public static List<ModelLine> ModelLines = new List<ModelLine>();
        public static List<ElementId> addedIds = new List<ElementId>();
        public static Document _doc = default(Document);

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;

            _doc = doc;

            doc.Application.DocumentChanged += OnDocumentChanged;

            ResultShow resultShowIn = ResultShow.Instance;
            resultShowIn.Helper().Owner = RevitWindowHelper.GetRevitHandle();
            resultShowIn.Show();

            uiapp.PostCommand(RevitCommandId.LookupPostableCommandId(PostableCommand.ModelLine));

            return Result.Succeeded;
        }

        private void OnDocumentChanged(object sender, DocumentChangedEventArgs e)
        {
            var app = sender as Application;
            var ids = e.GetAddedElementIds();
            foreach (var elementId in ids)
            {
                if (!addedIds.Contains(elementId))
                {
                    addedIds.Add(elementId);
                }
            }
        }
    }
}