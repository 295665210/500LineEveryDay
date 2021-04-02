using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CodeInBuildingCoder1
{
    [Transaction(TransactionMode.Manual)]
    class CmdFlatten : IExternalCommand
    {
        private const string _direct_shape_appGUID = "Flatten";

        Result Flatten(Document doc, ElementId viewId)
        {
            FilteredElementCollector col =
                new FilteredElementCollector(doc, viewId)
                    .WhereElementIsNotElementType();
            Options geometryOptions = new Options();
            using (Transaction tx = new Transaction(doc))
            {
                if (tx.Start("Convert elements to DirectShapes")
                    == TransactionStatus.Started)
                {
                    foreach (Element e in col)
                    {
                        GeometryElement gelt = e.get_Geometry(geometryOptions);
                        if (null != gelt)
                        {
                            string appDataGUID = e.Id.ToString();
                            //Currently create direct shape
                            //replacement element in the original
                            //document - no API to properly transfer
                            //graphic styles to a new document.
                            //A possible enhancement: make a copy
                            //of current project and operate on the copy.
                            DirectShape ds =
                                DirectShape.CreateElement(doc, e.Category.Id);
                            ds.ApplicationId = _direct_shape_appGUID;
                            ds.ApplicationDataId = appDataGUID;

                            try
                            {
                                ds.SetShape(new List<GeometryObject>(gelt));
                                doc.Delete(e.Id);
                            }
                            catch (Autodesk.Revit.Exceptions.ArgumentException
                                ex)
                            {
                                Debug
                                    .Print("Failed to replace {0};exception {1} {2}",
                                           Util.ElementDescription(e),
                                           ex.GetType().FullName, ex.Message);
                            }
                        }
                    }
                }
                tx.Commit();
            }
            return Result.Succeeded;
        }

        public Result Execute(
            ExternalCommandData commandData, ref string message,
            ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            // At the moment we convert to DirectShapes 
            // "in place" - that lets us preserve GStyles 
            // referenced by element shape without doing 
            // anything special.

            return Flatten(doc, uidoc.ActiveView.Id);
        }
        ///????这玩意干啥的？
    }
}