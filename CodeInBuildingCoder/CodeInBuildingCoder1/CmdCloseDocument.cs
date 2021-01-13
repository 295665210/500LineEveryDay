using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CodeInBuildingCoder1
{
    /// <summary>
    /// 
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    class CmdCloseDocument : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData, ref string message,
            ElementSet elements)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(CloseDocProc));
            return Result.Succeeded;
        }

        static void CloseDocProc(object stateInfo)
        {
            try
            {
                //maybe we need some checks for the right 
                //document, but this is a simple sample...
                SendKeys.SendWait("^{F4}");
            }
            catch (Exception e)
            {
                Util.ErrorMsg(e.Message);
            }
        }

        static void CloseDocByCommand(UIApplication uiapp)
        {
            RevitCommandId closeDoc =
                RevitCommandId.LookupPostableCommandId(PostableCommand.Close);
            uiapp.PostCommand(closeDoc);
        }
    }
}