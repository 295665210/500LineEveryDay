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

namespace DuplicateViews
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
            CreateCopyPastePanel(application);
            return Result.Succeeded;
        }

        private void CreateCopyPastePanel(UIControlledApplication application)
        {
            RibbonPanel rp = application.CreateRibbonPanel("CopyPaste");

            PushButtonData pbd2 = new PushButtonData("DuplicateAll", "Duplicate across documents", addAssemblyPath,
                typeof(DuplicateAcrossDocumentsCommand).FullName);
            pbd2.LongDescription = "Duplicate all duplicatable drafting views and schedules.";

            PushButton duplicateAllPB = rp.AddItem(pbd2) as PushButton;
            SetIconsForPushButton(duplicateAllPB, DuplicateViews.Properties.Resources.ViewCopyAcrossFiles);
        }

        private static void SetIconsForPushButton(PushButton button, Icon icon)
        {
            button.LargeImage = GetStdIcon(icon);
            button.Image = GetSmallIcon(icon);
        }

        private static BitmapSource GetSmallIcon(Icon icon)
        {
            Icon smallIcon = new System.Drawing.Icon(icon, new System.Drawing.Size(16, 16));
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                smallIcon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

        private static BitmapSource GetStdIcon(Icon icon)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }


        private static string addAssemblyPath = typeof(Application).Assembly.Location;
    }
}