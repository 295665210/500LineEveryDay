using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;

namespace CodeInBuildingCoder1.LinkCadTextToModelText
{
    internal class Start : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            Result result;
            try
            {
                application.CreateRibbonTab("Test");
                goto IL_21;
            }
            catch (Exception e)
            {
                "wrong".ShowTaskDialog();
                result = Result.Succeeded;
            }
            return result;

            IL_21:
            //add pannel
            RibbonPanel ribbonPanel = application.CreateRibbonPanel("Test", "Test1");
            //add item
            ribbonPanel.AddItem(new PushButtonData("CreateModelText", "转cad文字", Start.addinpath, "CodeInBuildingCoder1.CreateModelText")
            {
                // LargeImage = new BitmapImage(new Uri(快速弹夹.Imagepath + "changeColor32.png")),
                ToolTip = "cad文字转Revit文字"
            });


            return Result.Succeeded;
        }

        private static string addinpath = typeof(Start).Assembly.Location;


        private static string Imagepath =
            "C:\\ProgramData\\Autodesk\\Revit\\Addins\\2018\\icon\\";
    }
}