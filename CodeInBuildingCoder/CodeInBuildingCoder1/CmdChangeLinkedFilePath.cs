using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;

namespace CodeInBuildingCoder1
{
    /// <summary>
    /// This command will change the path of al linked Revit
    /// files the next time th document at the given location
    /// is opened.Please refer to the TransmissionData reference
    /// for more details
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CmdChangeLinkedFilePath : IExternalCommand

    {
        public Result Execute(
            ExternalCommandData commandData, ref string message,
            ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Application app = uiapp.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            FilePath location = new FilePath("C:/file.rvt");

            TransmissionData transData =
                TransmissionData.ReadTransmissionData(location);

            if (null != transData)
            {
                //collect all (immediate) external 
                //references in the model
                ICollection<ElementId> externalReference =
                    transData.GetAllExternalFileReferenceIds();
                //find every reference that is a link;
                foreach (ElementId refId in externalReference)
                {
                    ExternalFileReference exRef =
                        transData.GetLastSavedReferenceData(refId);

                    if (exRef.ExternalFileReferenceType ==
                        ExternalFileReferenceType.RevitLink)
                    {
                        //change the path of the linked file,
                        //leaving everything else unchanged.
                        transData.SetDesiredReferenceData(refId,
                         new FilePath($"C:/MyNewPath/cut.rvt"),
                         exRef.PathType,
                         true);
                    }
                }

                //make sure the IsTransmitted property is set.
                transData.IsTransmitted = true;

                //modified transmission data must be saved back to the model.
                TransmissionData.WriteTransmissionData(location, transData);
            }
            else
            {
                TaskDialog.Show("Unload Links",
                                "The document does not have" +
                                " any transmission data.");
            }
            return Result.Succeeded;
        }
    }
}