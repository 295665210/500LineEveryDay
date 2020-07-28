using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.RevitAddIns;

namespace CancelSave
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class CancelSave : IExternalApplication
    {
        //member variables
        private const string thisAddinFileName = "CancelSave.addin";
        //the dictionary contains document hashcode and its original "Project Status" pair.
        Dictionary<int, string> documentOriginalStatusDic = new Dictionary<int, string>();
        private int hashcodeofCurrentClosingDoc;

        public Result OnStartup(UIControlledApplication application)
        {
            //subscribe to DocumentOpened,DocumentCreated,DocumentSaving and DocumentSavingAs events
            application.ControlledApplication.DocumentOpened +=
                new EventHandler<DocumentOpenedEventArgs>(ReservePojectOriginalStatus);
            application.ControlledApplication.DocumentCreated +=
                new EventHandler<DocumentCreatedEventArgs>(ReservePojectOriginalStatus);
            application.ControlledApplication.DocumentSaving +=
                new EventHandler<DocumentSavingEventArgs>(CheckProjectStatusUpdate);
            application.ControlledApplication.DocumentSavingAs +=
                new EventHandler<DocumentSavingAsEventArgs>(CheckProjectStatusUpdate);
            application.ControlledApplication.DocumentClosing +=
                new EventHandler<DocumentClosingEventArgs>(MemClosingDocumentHashCode);
            application.ControlledApplication.DocumentClosed +=
                new EventHandler<DocumentClosedEventArgs>(RemoveStatusofClosedDocument);
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            //unsubscribe to DocumentOpened,DocumentCreated,DocumentSaving and DocumentSavingAs events
            application.ControlledApplication.DocumentOpened -=
                new EventHandler<DocumentOpenedEventArgs>(ReservePojectOriginalStatus);
            application.ControlledApplication.DocumentCreated -=
                new EventHandler<DocumentCreatedEventArgs>(ReservePojectOriginalStatus);
            application.ControlledApplication.DocumentSaving -=
                new EventHandler<DocumentSavingEventArgs>(CheckProjectStatusUpdate);
            application.ControlledApplication.DocumentSavingAs -=
                new EventHandler<DocumentSavingAsEventArgs>(CheckProjectStatusUpdate);

            return Result.Succeeded;
        }

#region EventHandler
        private void ReservePojectOriginalStatus(object sender, RevitAPIPostDocEventArgs args)
        {
            //the document associated with the event.Here means which document has been created or opened.
            Document doc = args.Document;
            //project information is unavailable for Family document.
            if (doc.IsFamilyDocument)
            {
                return;
            }

            LogManager.WriteLog(args, doc);
            int docHashCode = doc.GetHashCode();
            string currentProjectStatus = RetrieveProjectCurrentStatus(doc);
            documentOriginalStatusDic.Add(docHashCode, currentProjectStatus);
            LogManager.WriteLog("   Current Project Status: " + currentProjectStatus);
        }

        private void CheckProjectStatusUpdate(object sender, RevitAPIPreDocEventArgs args)
        {
            Document doc = args.Document;
            if (doc.IsFamilyDocument)
            {
                return;
            }
            LogManager.WriteLog(args, doc);
            string currentProjectStatus = RetrieveProjectCurrentStatus(args.Document);

            string originalProjectStatus = documentOriginalStatusDic[doc.GetHashCode()];

            LogManager.WriteLog("   Current Project Status: " + currentProjectStatus + "; Original Project Status: " +
                originalProjectStatus);

            if (string.IsNullOrEmpty(currentProjectStatus) && string.IsNullOrEmpty(originalProjectStatus)
                || (0 == string.Compare(currentProjectStatus, originalProjectStatus, true)))
            {
                DealNotUpdate(args);
                return;
            }

            documentOriginalStatusDic.Remove(doc.GetHashCode());
            documentOriginalStatusDic.Add(doc.GetHashCode(), currentProjectStatus);
        }

        private void MemClosingDocumentHashCode(object sender, DocumentClosingEventArgs args)
        {
            hashcodeofCurrentClosingDoc = args.DocumentId.GetHashCode();
        }

        private void RemoveStatusofClosedDocument(object sender, DocumentClosedEventArgs args)
        {
            if (args.Status.Equals(RevitAPIEventStatus.Succeeded) &&
                (documentOriginalStatusDic.ContainsKey(hashcodeofCurrentClosingDoc)))
            {
                documentOriginalStatusDic.Remove(hashcodeofCurrentClosingDoc);
            }
        }
#endregion


        //if the event is cancellable ,cancel it and inform user else just inform user the status.

        public static void DealNotUpdate(RevitAPIPreDocEventArgs args)
        {
            string mainMessage;
            string additionalText;
            TaskDialog taskDialog = new TaskDialog("CancelSave Sample");

            if (args.Cancellable)
            {
                args.Cancel();
                mainMessage = " CancelSave sample detected that the project status parameter on project info has not" +
                    "been updated. the file will not be saved.";
            }
            else
            {
                //will not cancel this event since it isn't cancellable
                mainMessage =
                    "this file is about to save. But cancelsave sample detected that the project status parameter" +
                    "on project info has not been updated.";
            }

            // taskDialog will not show when do regression test.
            if (!LogManager.RegressionTestNow)
            {
                additionalText =
                    "You can disable this permanently by uninstaling the CancelSave sample from Revit. Remove or rename CancelSave.addin from the addins directory.";

                // use one taskDialog to inform user current situation.     
                taskDialog.MainInstruction = mainMessage;
                taskDialog.MainContent = additionalText;
                taskDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, "Open the addins directory");
                taskDialog.CommonButtons = TaskDialogCommonButtons.Close;
                taskDialog.DefaultButton = TaskDialogResult.Close;
                TaskDialogResult tResult = taskDialog.Show();
                if (TaskDialogResult.CommandLink1 == tResult)
                {
                    System.Diagnostics.Process.Start("explorer.exe",
                        DetectAddinFileLocation(args.Document.Application));
                }
            }
            // write log file.
            LogManager.WriteLog("   Project Status is not updated, taskDialog informs user: " + mainMessage);
        }

        private static string RetrieveProjectCurrentStatus(Document doc)
        {
            // Project information is unavailable for Family document.
            if (doc.IsFamilyDocument)
            {
                return null;
            }

            // get project status stored in project information object and return it.
            return doc.ProjectInformation.Status;
        }

        private static string DetectAddinFileLocation(Autodesk.Revit.ApplicationServices.Application applictaion)
        {
            string addinFileFolderLocation = null;
            IList<RevitProduct> installedRevitList = RevitProductUtility.GetAllInstalledRevitProducts();

            foreach (RevitProduct revit in installedRevitList)
            {
                if (revit.Version.ToString().Contains(applictaion.VersionNumber))
                {
                    string allUsersAddInFolder = revit.AllUsersAddInFolder;
                    string currentUserAddInFolder = revit.CurrentUserAddInFolder;

                    if (File.Exists(Path.Combine(allUsersAddInFolder, thisAddinFileName)))
                    {
                        addinFileFolderLocation = allUsersAddInFolder;
                    }
                    else if (File.Exists(Path.Combine(currentUserAddInFolder, thisAddinFileName)))
                    {
                        addinFileFolderLocation = currentUserAddInFolder;
                    }

                    break;
                }
            }

            return addinFileFolderLocation;
        }
    }
}