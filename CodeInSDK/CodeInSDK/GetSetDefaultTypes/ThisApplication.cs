using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using CodeInSDK.GetSetDefaultTypes.CS;

namespace CodeInSDK.GetSetDefaultTypes
{
    public class ThisApplication : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                string str = "Default Type Selector";
                RibbonPanel panel = application.CreateRibbonPanel(str);
                string directionName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                PushButtonData data = new PushButtonData("Default Type Selector", "Default Type Selector",
                    directionName + @"\GetSetDefaultTypes.dll", "CodeInSDK.GetSetDefaultTypes.ThisCommand");
                PushButton button = panel.AddItem(data) as PushButton;
                button.LargeImage = new BitmapImage(new Uri(directionName + "\\Resources\\type.png"));

                //register dockable windows on startup
                DefaultFamilyTypesPane = new DefaultFamilyTypes();
                DefaultElementTypesPane = new DefaultElementTypes();

                application.RegisterDockablePane(DefaultFamilyTypes.PaneId, "Default Family Types",
                    DefaultFamilyTypesPane);
                application.RegisterDockablePane(DefaultElementTypes.PaneId, "Default Element Types",
                    DefaultElementTypesPane);

                //register view active event
                application.ViewActivated += new EventHandler<ViewActivatedEventArgs>(application_ViewActivated);
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Default type selector");
                return Result.Failed;
            }
        }


        public static DefaultFamilyTypes DefaultFamilyTypesPane;
        public static DefaultElementTypes DefaultElementTypesPane;

        void application_ViewActivated(object sender, ViewActivatedEventArgs e)
        {
            if (!DockablePane.PaneExists(DefaultFamilyTypes.PaneId) ||
                !DockablePane.PaneExists(DefaultElementTypes.PaneId))
            {
                return;
            }

            UIApplication uiApp = sender as UIApplication;
            if (uiApp == null)
            {
                return;
            }

            if (DefaultFamilyTypesPane != null)
            {
                DefaultFamilyTypesPane.SetDocument(e.Document);
            }

            if (DefaultElementTypesPane != null)
            {
                DefaultElementTypesPane.SetDocument(e.Document);
            }
        }
    }
}