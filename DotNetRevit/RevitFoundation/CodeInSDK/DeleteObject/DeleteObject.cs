using System.Collections;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitFoundation.CodeInSDK.DeleteObject
{
    /// <summary>
    /// Delete the elements that were selected
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class DeleteObject : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Transaction trans = new Transaction(uiapp.ActiveUIDocument.Document, "DeleteObject");
            trans.Start();
            ElementSet collection = new ElementSet();
            foreach (ElementId elementId in uiapp.ActiveUIDocument.Selection.GetElementIds())
            {
                collection.Insert(uiapp.ActiveUIDocument.Document.GetElement(elementId));
            }

            //check user selection
            if (collection.Size < 1)
            {
                message = "Please select object(s) before delete.";
                trans.RollBack();
                return Result.Cancelled;
            }

            bool error=true;
            try
            {
                // error = true;

                //delete selection
                IEnumerator e = collection.GetEnumerator();
                bool moreValue = e.MoveNext();
                while (moreValue)
                {
                    Element component = e.Current as Element;
                    uiapp.ActiveUIDocument.Document.Delete(component.Id);
                    moreValue = e.MoveNext();
                }
                error = false;
            }
            catch 
            {
                // if revit throw an exception, tyr to catch it
                foreach (Element c in collection)
                {
                    elements.Insert(c);
                }
                message = "object(s) can't be deleted.";
                trans.RollBack();
                return Result.Failed;
            }

            finally
            {
                //if revit threw an exception,display error and return failed 
                if (error)
                {
                    TaskDialog.Show("Error", "Delete failed.");
                }
            }

            trans.Commit();
            return Result.Succeeded;
        }
    }
}