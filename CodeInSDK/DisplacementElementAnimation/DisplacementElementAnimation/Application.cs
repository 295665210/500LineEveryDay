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

namespace DisplacementElementAnimation
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Application : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            CreateDisplacementPanel(application);

            return Result.Succeeded;
        }


        private void CreateDisplacementPanel(UIControlledApplication application)
        {
            RibbonPanel rp = application.CreateRibbonPanel("Displacement");
            PushButtonData setupMonitor = new PushButtonData("Displacement_Animation", "Displacement_Animation",
                addAssemblyPath,
                typeof(DisplacementElementAnimation.DisplacementStructureModelAnimatorCommand)
                    .FullName);
            PushButton setupMonitorPB = rp.AddItem(setupMonitor) as PushButton;
            SetIconsForPushButton(setupMonitorPB, DisplacementElementAnimation.Properties.Resources.DisplacementPlay);
        }

        public static void SetIconsForPushButton(PushButton button, System.Drawing.Icon icon)
        {
            button.LargeImage = GetStdIcon(icon);
            button.Image = GetSmallIcon(icon);
        }

        private static BitmapSource GetStdIcon(System.Drawing.Icon icon)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        private static BitmapSource GetSmallIcon(Icon icon)
        {
            Icon smallIcon = new Icon(icon, new System.Drawing.Size(16, 16));
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(smallIcon.Handle, Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }


        static String addAssemblyPath = typeof(DisplacementElementAnimation.Application).Assembly.Location;
    }
}