using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Document = Autodesk.Revit.DB.Document;

namespace CodeInBuildingCoder1
{
    [Transaction(TransactionMode.Manual)]
    class CmdDeleteUnusedRefPlanes : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData, ref string message,
            ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            var app = uiapp.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            FilteredElementCollector filet = new FilteredElementCollector(doc);
            List<ElementId> refIds = filet.OfClass(typeof(ReferencePlane))
                .ToElementIds().ToList();

            using (TransactionGroup tg = new TransactionGroup(doc))
            {
                tg.Start("Remove Un-Used Reference Planes");
                foreach (ElementId id in refIds)
                {
                    var filt2 = new ElementClassFilter(typeof(FamilyInstance));
                    var filt3 =
                        new ElementParameterFilter(new
                                                       FilterElementIdRule(new ParameterValueProvider(new ElementId(BuiltInParameter.HOST_ID_PARAM)),
                                                        new
                                                            FilterNumericEquals(),
                                                        id));
                    var filt4 = new LogicalAndFilter(filt2, filt3);
                    var thing = new FilteredElementCollector(doc);

                    using (Transaction t = new Transaction(doc))
                    {
                        if (!thing.WherePasses(filt4).Any())
                        {
                            t.Start("Do The Thing");

#if Revit2018
  if (doc.GetElement(id).GetDependentElements(new ElementClassFilter(typeof(FamilyInstance))).Count == 0)
                        {
                            doc.Delete(id);
                        }
                        t.Commit();
#endif

                            // Make sure there is nothing measuring to the plane
                            if (doc.Delete(id).Count() > 1)
                            {
                                t.Dispose();
                            }
                            else
                            {
                                t.Commit();
                            }
                        }
                        else
                        {
                            //skiped
                        }
                    }
                }
                tg.Assimilate();
            }
            return Result.Succeeded;
        }
    }
}