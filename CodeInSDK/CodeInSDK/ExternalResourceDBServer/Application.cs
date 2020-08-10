// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
// using Autodesk.Revit.ApplicationServices;
// using Autodesk.Revit.DB;
// using Autodesk.Revit.DB.ExternalService;
// using Autodesk.Revit.UI;
//
// namespace CodeInSDK.ExternalResourceDBServer
// {
//     public class DBApplication : IExternalDBApplication
//     {
// #region IExternaDBApplication Members
//         public ExternalDBApplicationResult OnStartup(ControlledApplication application)
//         {
//             //get revit's ExternalResourceService
//             ExternalService externalResourceService =
//                 ExternalServiceRegistry.GetService(ExternalServices.BuiltInExternalServices.ExternalResourceService);
//
//             if (externalResourceService == null)
//             {
//                 return ExternalDBApplicationResult.Failed;
//             }
//
//             //create an instance of your IExternalResourceServer and register it with the ExternalResourceService.
//             IExternalResourceServer sampleServer = new SampleExternalResourceDBServer();
//             externalResourceService.AddServer(sampleServer);
//             return ExternalDBApplicationResult.Succeeded;
//         }
//
//         public ExternalDBApplicationResult OnShutdown(ControlledApplication application)
//         {
//             return ExternalDBApplicationResult.Succeeded;
//         }
// #endregion
//     }
// }