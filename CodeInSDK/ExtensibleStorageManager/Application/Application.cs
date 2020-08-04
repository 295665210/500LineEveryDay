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

namespace ExtensibleStorageManager
{
    public class Application : IExternalApplication
    {
        /// <summary>
        /// There is no cleanup needed in this application  -- default implementation
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        /// <summary>
        /// Add a button to Ribbon and attach it to the IExternalCommand defined in Command.CS
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        public Result OnStartup(UIControlledApplication application)
        {
            RibbonPanel rp = application.CreateRibbonPanel("Extensible Storage Manager");
            string currentAssembly = System.Reflection.Assembly.GetAssembly(this.GetType()).Location;

            PushButton pb = rp.AddItem(
                new PushButtonData("Extensible Storage Manager",
                    "Extensible Storage Manager",
                    currentAssembly,
                    "ExtensibleStorageManager.Command")
            ) as PushButton;

            return Result.Succeeded;
        }
    }
}