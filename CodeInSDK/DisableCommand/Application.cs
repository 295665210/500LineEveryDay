using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;

namespace DisableCommand
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Application : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            //lookup the desired command by name
            s_commandId = RevitCommandId.LookupCommandId(s_commandToDisable);

            //confirm that the command can be overridden
            if (!s_commandId.CanHaveBinding)
            {
                ShowDialog("Error",
                    "the target command" + s_commandToDisable + "selected for disabling cannot be overriden");
                return Result.Failed;
            }

            try
            {
                AddInCommandBinding commandBinding = application.CreateAddInCommandBinding(s_commandId);
                commandBinding.Executed += DisableEvent;
            }
            catch (Exception)
            {
                ShowDialog("Error",
                    "this add-in is unable to disable the target commadn" + s_commandToDisable +
                    "; most likely another add-in has overridden this command.");
            }
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            // remove the command binding on shutdown
            if (s_commandId.HasBinding)
            {
                application.RemoveAddInCommandBinding(s_commandId);
            }
            return Result.Succeeded;
        }


        private void DisableEvent(object sender, ExecutedEventArgs args)
        {
            ShowDialog("Disabled", "use of this command has been disabled");
        }

        private static void ShowDialog(string tittle, string message)
        {
            TaskDialog td = new TaskDialog(tittle)
            {
                MainInstruction = message,
                TitleAutoPrefix = false
            };
            td.Show();
        }

        static String s_commandToDisable = "ID_EDIT_DESIGNOPTIONS";

        static RevitCommandId s_commandId;
    }
}