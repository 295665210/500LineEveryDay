using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace HiTools
{
    [Transaction(TransactionMode.Manual)]
    class ApplicationMain : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            #region panel-1
            //1 创建RibbonTab
            application.CreateRibbonTab("HiTools");
            //2 在tab中创建uiPanel
            RibbonPanel ribbonPanel1 = application.CreateRibbonPanel("HiTools", "UIPannel");
            //3 指定程序集的名称及所使用的类名
            string assemblyPath = @"D:\Git\githubRep2\Gitee500LinesEveryday\Tools\HiTools\bin\Debug\HiTools.dll";
            string classNameHelloRevit = "HiTools.Cmd_HelloRevit";
            //4 创建按钮 pushButton
            PushButtonData pbd =
                new PushButtonData("HiToolsInternalName", "HelloRevit", assemblyPath, classNameHelloRevit);
            PushButton pushButton = ribbonPanel1.AddItem(pbd) as PushButton;
            //4-2 给按钮加个图片: 支持png, 大小要求32x32 16x16
            // string imgPath = @"D:\Desktop\hello32x32.png";
            // pushButton.LargeImage =new BitmapImage(new Uri(imgPath));
            pushButton.LargeImage =
                new BitmapImage(new Uri("pack://application:,,,/HiTools;component/Source/HelloRevit32x32.png",
                                        UriKind.Absolute));
            //4-3 给按钮设置一个默认的提示信息
            pushButton.ToolTip = "Hello Revit";

            //4-4 再加一个button HelloWorld;
            string classNameHelloWorld = "HiTools.Cmd_HelloWorld";
            PushButtonData pbd2 =
                new PushButtonData("HiToolsInternalName2", "HelloWorld", assemblyPath, classNameHelloWorld);
            PushButton pushButton2 = ribbonPanel1.AddItem(pbd2) as PushButton;
            pushButton2.LargeImage =
                new BitmapImage(new Uri("pack://application:,,,/HiTools;component/Source/HelloWorld32x32.png",
                                        UriKind.Absolute));
            pushButton2.ToolTip = "Hello World";

            //4-5 再加一个button: Cmd_PickBox3D ;
            string classNameCmd_PickBox3D = "HiTools.Cmd.Cmd_PickBox3D";
            PushButtonData pbd_Cmd_PickBox3D =
                new PushButtonData("Cmd_PickBox3D", "PickBox3D", assemblyPath, classNameCmd_PickBox3D);
            PushButton pushButton_Cmd_PickBox3D = ribbonPanel1.AddItem(pbd_Cmd_PickBox3D) as PushButton;
            pushButton_Cmd_PickBox3D.LargeImage =
                new BitmapImage(new Uri("pack://application:,,,/HiTools;component/Source/Cmd_PickBox3D.png",
                                        UriKind.Absolute));
            pushButton_Cmd_PickBox3D.ToolTip = "Cmd_PickBox3D";

            //4-6 再加一个button: Cmd_PickBox3D ;
            string className_Cmd_ReverseBackGroundColor = "HiTools.Cmd.Cmd_ReverseBackGroundColor";
            PushButtonData pbd_Cmd_ReverseBackGroundColor =
                new PushButtonData("Cmd_ReverseBackGroundColor", "ReverseBGColor", assemblyPath,
                                   className_Cmd_ReverseBackGroundColor);
            PushButton pushButton_Cmd_ReverseBackGroundColor =
                ribbonPanel1.AddItem(pbd_Cmd_ReverseBackGroundColor) as PushButton;
            pushButton_Cmd_ReverseBackGroundColor.LargeImage =
                new BitmapImage(new
                                    Uri("pack://application:,,,/HiTools;component/Source/Cmd_ReverseBackGroundColor.png",
                                        UriKind.Absolute));
            pushButton_Cmd_ReverseBackGroundColor.ToolTip = "Cmd_PickBox3D";
            #endregion

            // //注意要点:
            // //1 当前程序集路径
            // string InfoAssemblyPath = Assembly.GetExecutingAssembly().Location;
            // //1.2 当前程序的路径
            // string InfoRevitLocation = Process.GetCurrentProcess().MainModule.FileName;

            #region panel-2
            //2 在tab中再创建uiPanel StackPanel
            RibbonPanel rp2 = application.CreateRibbonPanel("HiTools", "StackedItem");

            string Cmd_UnhideElementsInFamlyDoc = "HiTools.Cmd.Cmd_UnhideElementsInFamlyDoc";
            RibbonItemData item1 =
                new PushButtonData("Cmd_UnhideElementsInFamlyDoc", "UnHideEInFamly", assemblyPath,
                                   Cmd_UnhideElementsInFamlyDoc);

            string Cmd_HideElementInfamilyDoc = "HiTools.Cmd.Cmd_HideElementInFamilyDoc";
            RibbonItemData item2 =
                new PushButtonData("Cmd_HideElementInFamilyDoc", "HideEInFamly", assemblyPath,
                                   Cmd_HideElementInfamilyDoc);

            string Cmd_ForcedDisplayInDoc = "HiTools.Cmd.Cmd_ForcedDisplayInDoc";
            RibbonItemData item3 =
                new PushButtonData("Cmd_ForcedDisplayInDoc", "DisplayInDoc", assemblyPath,
                                   Cmd_ForcedDisplayInDoc);

            IList<RibbonItem> listRibbon1 = rp2.AddStackedItems(item1, item2, item3);

            //加图片和提示
            PushButton pushButton3 = listRibbon1[0] as PushButton;
            pushButton3.Image =
                new BitmapImage(new
                                    Uri("pack://application:,,,/HiTools;component/Source/Cmd_UnhideElementsInDoc.png",
                                        UriKind.Absolute));
            pushButton3.ToolTip = "Cmd_UnhideElementsInFamlyDoc";

            //
            PushButton pushButton4 = listRibbon1[1] as PushButton;
            pushButton4.Image =
                new BitmapImage(new
                                    Uri("pack://application:,,,/HiTools;component/Source/Cmd_HideElementInFamilyDoc.png",
                                        UriKind.Absolute));
            pushButton4.ToolTip = "Cmd_HideElementInfamilyDoc";

            //
            PushButton pushButton5 = listRibbon1[2] as PushButton;
            pushButton5.Image =
                new BitmapImage(new
                                    Uri("pack://application:,,,/HiTools;component/Source/Cmd_ForcedDisplayInDoc.png",
                                        UriKind.Absolute));
            pushButton4.ToolTip = "Cmd_ForcedDisplayInDoc";
            #endregion

            #region panel-3
         
            //2 在tab中创建Panel -view
            RibbonPanel ribbonPanel3 = application.CreateRibbonPanel("HiTools", "View");
            //3 指定程序集的名称及所使用的类名
            string classNameViewSimu = "HiTools.Cmd.Cmd_ViewSimultaneous";
            //4 创建按钮 pushButton
            PushButtonData pbd31 =
                new PushButtonData("HiToolsInternalName", "ViewSimu", assemblyPath, classNameViewSimu);
            PushButton pushButton31 = ribbonPanel3.AddItem(pbd31) as PushButton;
            //4-2 给按钮加个图片: 支持png, 大小要求32x32 16x16
            // string imgPath = @"D:\Desktop\hello32x32.png";
            // pushButton.LargeImage =new BitmapImage(new Uri(imgPath));
            pushButton31.LargeImage =
                new BitmapImage(new Uri("pack://application:,,,/HiTools;component/Source/Cmd_ViewSimultaneous32x32.png",
                                        UriKind.Absolute));
            //4-3 给按钮设置一个默认的提示信息
            pushButton31.ToolTip = "视图同步";

           

            

           
            #endregion

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}