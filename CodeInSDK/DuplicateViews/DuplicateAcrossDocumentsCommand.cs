using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace DuplicateViews
{
    [Transaction(TransactionMode.Manual)]
    class DuplicateAcrossDocumentsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Autodesk.Revit.ApplicationServices.Application application = commandData.Application.Application;
            Document doc = commandData.Application.ActiveUIDocument.Document;

            //find target document - it must be only other open document in session.
            Document toDocument = null;
            IEnumerable<Document> documents = application.Documents.Cast<Document>();
            if (documents.Count<Document>() != 2)
            {
                TaskDialog.Show("No target document",
                    "This tools can only be used if there are two documents (a source document and target docuement).");
                return Result.Cancelled;
            }

            foreach (Document loadedDoc in documents)
            {
                if (loadedDoc.Title != doc.Title)
                {
                    toDocument = loadedDoc;
                    break;
                }
            }

            //collect schedules and drafting views
            FilteredElementCollector collector = new FilteredElementCollector(doc);

            List<Type> viewTypes = new List<Type>();
            viewTypes.Add(typeof(ViewSchedule));
            viewTypes.Add(typeof(ViewDrafting));
            ElementMulticlassFilter filter = new ElementMulticlassFilter(viewTypes);
            collector.WherePasses(filter);
            collector.WhereElementIsViewIndependent(); //skip view-specfic schedules(e.g. Revisino Schedules);
            //These should not be copied as they are associated to another view that cannot be copied
            //copy all schedules together so that any dependency elements are copied only once.
            IEnumerable<ViewSchedule> schedules = collector.OfType<ViewSchedule>();
            DuplicateViewUtils.DuplicateSchedules(doc, schedules, toDocument);
            int numSchedules = schedules.Count<ViewSchedule>();

            //copy drafting views together
            IEnumerable<ViewDrafting> draftingViews = collector.OfType<ViewDrafting>();
            int numDraftingElements = DuplicateViewUtils.DuplicateDraftingViews(doc, draftingViews, toDocument);
            int numDrafting = draftingViews.Count<ViewDrafting>();

            //show results
            TaskDialog.Show("Statistics",
                String.Format("Copied: \n" +
                    "\t{0} schedules.\n" +
                    "\t{1}drafting views.\n" +
                    "\t{2}new dafting elements created.", numSchedules, numDrafting, numDrafting, numDraftingElements));
            return Result.Succeeded;
        }
    }
}