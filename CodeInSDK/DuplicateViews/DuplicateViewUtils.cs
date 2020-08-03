using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using View = Autodesk.Revit.DB.View;

namespace DuplicateViews
{
    class DuplicateViewUtils
    {
        /// <summary>
        /// Utility to duplicate schedules from one document to another.
        /// </summary>
        /// <param name="fromDocument"></param>
        /// <param name="views"></param>
        /// <param name="toDocument"></param>
        public static void DuplicateSchedules
            (Document fromDocument, IEnumerable<ViewSchedule> views, Document toDocument)
        {
            //Use LINQ to convert to list of ElementIds for use in CopyElement() method.
            List<ElementId> ids = views.AsEnumerable<View>().ToList<View>()
                .ConvertAll<ElementId>(ViewConvertToElementId);

            //Duplicate, Pass false to make the function skip returning the map from source element to its copy.
            DuplicateElementsAcrossDocuments(fromDocument, ids, toDocument, false);
        }


        public static int DuplicateDraftingViews
            (Document fromDocument, IEnumerable<ViewDrafting> views, Document toDocument)
        {
            //Return value
            int numberOfDetailElements = 0;
            //Transaction group for all activities
            using (TransactionGroup tg =
                new TransactionGroup(toDocument, "API - Duplication across documents with detailing "))
            {
                tg.Start();

                //Use LINQ to list of ElementIds for use in CopyElements() method
                List<ElementId> ids =
                    views.AsEnumerable<View>().ToList<View>().ToList().ConvertAll<ElementId>(ViewConvertToElementId);

                //Duplicate. Pass true to get a map from source element to its copy
                Dictionary<ElementId, ElementId> viewMap =
                    DuplicateElementsAcrossDocuments(fromDocument, ids, toDocument, true);

                //For each copied view,copy the contents
                foreach (ElementId viewId in viewMap.Keys)
                {
                    View fromView = fromDocument.GetElement(viewId) as View;
                    View toView = toDocument.GetElement(viewMap[viewId]) as View;
                    numberOfDetailElements += DuplicateDetailingAcrossViews(fromView, toView);
                }
                tg.Assimilate();
            }
            return numberOfDetailElements;
        }

        private static int DuplicateDetailingAcrossViews(View fromView, View toView)
        {
            //collect view-specific elements in source view
            FilteredElementCollector collector = new FilteredElementCollector(fromView.Document, fromView.Id);

            //skip elements which don't have a category.In testing,this 
            //was revision table and the extents element,which should not be copied as they will be automatically created for the copied view.
            collector.WherePasses(new ElementCategoryFilter(ElementId.InvalidElementId, true));

            //get collection of elements to copy for CopyElements()
            ICollection<ElementId> toCopy = collector.ToElementIds();

            //return vale
            int numberOfCopiedElements = 0;

            if (toCopy.Count > 0)
            {
                using (Transaction t2 = new Transaction(toView.Document, "Duplicate view detailing"))
                {
                    t2.Start();
                    //Set handler to skip the duplicate types dialog;
                    CopyPasteOptions options = new CopyPasteOptions();
                    options.SetDuplicateTypeNamesHandler(new HideAndAcceptDuplicateTypeNamesHandler());

                    //copy the elements using no transformation
                    ICollection<ElementId> copiedElements =
                        ElementTransformUtils.CopyElements(fromView, toCopy, toView, Transform.Identity, options);

                    numberOfCopiedElements = copiedElements.Count;

                    //set failure handler to skip any duplicate types warnings that are posted.
                    FailureHandlingOptions failureOptions = t2.GetFailureHandlingOptions();
                    failureOptions.SetFailuresPreprocessor(new HidePasteDuplicateTypesPreprocessor());
                    t2.Commit();
                }
            }
            return numberOfCopiedElements;
        }


        private static Dictionary<ElementId, ElementId> DuplicateElementsAcrossDocuments
            (Document fromDocument, ICollection<ElementId> elementIds, Document toDocument, bool findMatchingElements)
        {
            //return value.
            Dictionary<ElementId, ElementId> elementMap = new Dictionary<ElementId, ElementId>();

            ICollection<ElementId> copiedIds;
            using (Transaction t1 = new Transaction(toDocument, "Duplicate elements"))
            {
                t1.Start();

                //set options for copy-paste to hide the duplicate types dialog;
                CopyPasteOptions options = new CopyPasteOptions();
                options.SetDuplicateTypeNamesHandler(new HideAndAcceptDuplicateTypeNamesHandler());

                //copy the input elements.
                copiedIds = ElementTransformUtils.CopyElements(fromDocument, elementIds, toDocument, Transform.Identity,
                    options);

                //set failure handler to hide duplicate types warning which may be posted.
                FailureHandlingOptions failureOptions = t1.GetFailureHandlingOptions();
                failureOptions.SetFailuresPreprocessor(new HidePasteDuplicateTypesPreprocessor());
                t1.Commit(failureOptions);
            }

            //find matching elements if required.
            if (findMatchingElements)
            {
                //built a map from name ->source element
                Dictionary<String, ElementId> nameToFromElementsMap = new Dictionary<string, ElementId>();
                foreach (ElementId id in elementIds)
                {
                    Element e = fromDocument.GetElement(id);
                    String name = e.Name;
                    if (!String.IsNullOrEmpty(name))
                    {
                        nameToFromElementsMap.Add(name, id);
                    }
                }
                //built a map from name->target element
                Dictionary<String, ElementId> nameToToElementIdsMap = new Dictionary<string, ElementId>();

                foreach (ElementId id in copiedIds)
                {
                    Element e = toDocument.GetElement(id);
                    String name = e.Name;
                    if (!String.IsNullOrEmpty(name))
                    {
                        nameToToElementIdsMap.Add(name, id);
                    }
                }

                //merge to make source element ->target element map
                foreach (string name in nameToToElementIdsMap.Keys)
                {
                    ElementId copiedId;
                    if (nameToToElementIdsMap.TryGetValue(name,out copiedId))
                    {
                        elementMap.Add(nameToToElementIdsMap[name],copiedId);
                    }
                }
            }
            return elementMap;
        }

        private static ElementId ViewConvertToElementId(View view)
        {
            return view.Id;
        }
    }

    /// <summary>
    /// a handler to accept duplicate types names created by the copy/paste operation.
    /// </summary>
    class HideAndAcceptDuplicateTypeNamesHandler : IDuplicateTypeNamesHandler
    {
        public DuplicateTypeAction OnDuplicateTypeNamesFound(DuplicateTypeNamesHandlerArgs args)
        {
            //always use duplicate destination types when asked
            return DuplicateTypeAction.UseDestinationTypes;
        }
    }

    /// <summary>
    /// a failure preprocessor to hide the warning about duplicate types being pasted.
    /// </summary>
    class HidePasteDuplicateTypesPreprocessor : IFailuresPreprocessor
    {
        public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
        {
            foreach (FailureMessageAccessor failure in failuresAccessor.GetFailureMessages())
            {
                //delete any "Can't paste duplicate types.  Only non duplicate types will be pasted." warnings.
                if (failure.GetFailingElementIds() == BuiltInFailures.CopyPasteFailures.CannotCopyDuplicates)
                {
                    failuresAccessor.DeleteWarning(failure);
                }
            }
            return FailureProcessingResult.Continue;
        }
    }
}