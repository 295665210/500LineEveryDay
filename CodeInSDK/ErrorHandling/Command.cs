// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Security.AccessControl;
// using System.Text;
// using System.Threading.Tasks;
// using System.Windows.Forms;
// using Autodesk.Revit.Attributes;
// using Autodesk.Revit.DB;
// using Autodesk.Revit.UI;
// using Autodesk.Revit.UI.Selection;
// using Application = Autodesk.Revit.ApplicationServices.Application;
// using View = Autodesk.Revit.DB.View;
//
// namespace ErrorHandling
// {
//     [Transaction(TransactionMode.Manual)]
//     [Regeneration(RegenerationOption.Manual)]
//     [Journaling(JournalingMode.UsingCommandData)]
//     public class Command : IExternalCommand, IExternalApplication
//     {
//         /// <summary>
//         /// The failure definition id for warning.
//         /// </summary>
//         public static FailureDefinitionId m_idWarning;
//
//         /// <summary>
//         /// The failure definition id for error.
//         /// </summary>
//         public static FailureDefinitionId m_idError;
//
//         /// <summary>
//         /// The failure definition for warning .
//         /// </summary>
//         private FailureDefinition m_fdWarning;
//
//         /// <summary>
//         /// The failure definition for error
//         /// </summary>
//         private FailureDefinition m_fdError;
//
//         /// <summary>
//         /// The Revit application
//         /// </summary>
//         private Application m_revitApp;
//
//         /// <summary>
//         /// The active document
//         /// </summary>
//         private Document m_doc;
//
// #region IExternalApplication members
//         public Result OnStartup(UIControlledApplication application)
//         {
//             try
//             {
//                 //Create failure definition Ids
//                 Guid guid1 = new Guid("0C3F66B5-3E26-4d24-A228-7A8358C76D39");
//                 Guid guid2 = new Guid("93382A45-89A9-4cfe-8B94-E0B0D9542D34");
//                 Guid guid3 = new Guid("A16D08E2-7D06-4bca-96B0-C4E4CC0512F8");
//
//                 m_idWarning = new FailureDefinitionId(guid1);
//                 m_idError = new FailureDefinitionId(guid2);
//
//                 //Create failure definitions and add resolutions
//                 m_fdWarning =
//                     FailureDefinition.CreateFailureDefinition(m_idWarning, FailureSeverity.Warning,
//                         "I am the warning.");
//                 m_fdError = FailureDefinition.CreateFailureDefinition(m_idError, FailureSeverity.Error,
//                     "I am the error");
//
//                 m_fdWarning.AddResolutionType(FailureResolutionType.MoveElements, "MoveElements",
//                     typeof(DeleteElements));
//                 m_fdWarning.AddResolutionType(FailureResolutionType.DeleteElements, "DeleteElements",
//                     typeof(DeleteElements));
//                 m_fdWarning.SetDefaultResolutionType(FailureResolutionType.DeleteElements);
//                 m_fdError.AddResolutionType(FailureResolutionType.DetachElements, "DetachElements",
//                     typeof(DeleteElements));
//
//                 m_fdError.AddResolutionType(FailureResolutionType.DeleteElements, "DeleteElements",
//                     typeof(DeleteElements));
//
//                 m_fdError.SetDefaultResolutionType(FailureResolutionType.DeleteElements);
//             }
//             catch (Exception)
//             {
//                 return Result.Failed;
//             }
//             return Result.Succeeded;
//         }
//
//         public Result OnShutdown(UIControlledApplication application)
//         {
//             return Result.Succeeded;
//         }
// #endregion
//
// #region IExternalCommand members
//         public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
//         {
//             m_revitApp = commandData.Application.Application;
//             m_doc = commandData.Application.ActiveUIDocument.Document;
//
//             Level level1 = GetLevel();
//             if (level1 == null)
//             {
//                 throw new Exception("[ERROR] Failed to get level 1");
//             }
//
//             try
//             {
//                 // Post a warning and resolve it in FailurePreproccessor.
//                 try
//                 {
//                     Transaction transaction = new Transaction(m_doc, "Warning_FailurePreproccessor");
//                     FailureHandlingOptions options = transaction.GetFailureHandlingOptions();
//                     FailurePreproccessor preproccessor = new FailurePreproccessor();
//                     options.SetFailuresPreprocessor(preproccessor);
//                     transaction.SetFailureHandlingOptions(options);
//                     transaction.Start();
//                     FailureMessage fm = new FailureMessage(m_idWarning);
//                     m_doc.PostFailure(fm);
//                     transaction.Commit();
//                 }
//                 catch (Exception)
//                 {
//                     message = "Failed to commit transaction Warning_FailurePreproccessor";
//                     return Result.Failed;
//                 }
//
//                 //Dismiss the overlapped wall warning in FailurePerprocessor
//                 try
//                 {
//                     Transaction transaction = new Transaction(m_doc, "Warning_FailurePreproccessor_OverlappedWall");
//                     FailureHandlingOptions options = transaction.GetFailureHandlingOptions();
//                     FailurePreproccessor preproccessor = new FailurePreproccessor();
//                     options.SetFailuresPreprocessor(preproccessor);
//                     transaction.SetFailureHandlingOptions(options);
//                     transaction.Start();
//
//
//                 }
//                 catch (Exception)
//                 {
//                     Console.WriteLine(e);
//                     throw;
//                 }
//             }
//             catch (Exception e)
//             {
//                 message = "Failed to commit transaction Warning_FailurePreproccessor_OverlappedWall";
//                 return Result.Failed;
//             }
//
//             try
//             {
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine(e);
//                 throw;
//             }
//
//             return Result.Succeeded;
//         }
//     }
// #endregion
//
//     /// <summary>
//     /// Implements the interface IFailuresPreprocessor
//     /// </summary>
//     public class FailurePreproccessor : IFailuresPreprocessor
//     {
//         /// <summary>
//         /// This method is called when there have been failures found at the end of a transaction and Revit is about to start processing them.
//         /// 
//         /// </summary>
//         /// <param name="failuresAccessor">The Interface class that provides access to the failure information.</param>
//         /// <returns></returns>
//         public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
//         {
//             IList<FailureMessageAccessor> fmas = failuresAccessor.GetFailureMessages();
//             if (fmas.Count == 0)
//             {
//                 return FailureProcessingResult.Continue;
//             }
//
//             string transactionName = failuresAccessor.GetTransactionName();
//             if (transactionName.Equals("Warning_FailurePreprocessor"))
//             {
//                 foreach (FailureMessageAccessor fma in fmas)
//                 {
//                     FailureDefinitionId id = fma.GetFailureDefinitionId();
//                     if (id == Command.m_idWarning)
//                     {
//                         failuresAccessor.DeleteWarning(fma);
//                     }
//                 }
//                 return FailureProcessingResult.ProceedWithCommit;
//             }
//
//             else if (transactionName.Equals("Warning_FailurePreproccessor_OverlappedWall"))
//             {
//                 foreach (FailureMessageAccessor fma in fmas)
//                 {
//                     FailureDefinitionId id = fma.GetFailureDefinitionId();
//                     if (id == BuiltInFailures.OverlapFailures.WallsOverlap)
//                     {
//                         failuresAccessor.DeleteWarning(fma);
//                     }
//                 }
//                 return FailureProcessingResult.ProceedWithCommit;
//             }
//             else
//             {
//                 return FailureProcessingResult.Continue;
//             }
//         }
//     }
//
//
//     /// <summary>
//     /// Implements the interface 
//     /// </summary>
//     public class FailuresProcessor : IFailuresPreprocessor
//     {
//         public void Dismiss()
//         {
//         }
//
//
//         public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
//         {
//             IList<FailureMessageAccessor> fmas = failuresAccessor.GetFailureMessages();
//             if (fmas.Count == 0)
//             {
//                 return FailureProcessingResult.Continue;
//             }
//
//             String transactionName = failuresAccessor.GetTransactionName();
//             if (transactionName.Equals("Error_FailuresProcessor"))
//             {
//                 foreach (FailureMessageAccessor fma in fmas)
//                 {
//                     FailureDefinitionId id = fma.GetFailureDefinitionId();
//                     if (id == Command.m_idError)
//                     {
//                         failuresAccessor.ResolveFailure(fma);
//                     }
//                 }
//                 return FailureProcessingResult.ProceedWithCommit;
//             }
//             else
//             {
//                 return FailureProcessingResult.Continue;
//             }
//         }
//     }
// }