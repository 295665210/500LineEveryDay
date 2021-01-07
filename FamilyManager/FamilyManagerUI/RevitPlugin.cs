using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MaterialDesignThemes.Wpf;
using RevitStorage;
using QiShiLog.Log;


namespace FamilyManagerUI
{
    [Transaction(TransactionMode.Manual)]
    public class RevitPlugin : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //引入第三方库，方法，防止报错
            PlatformType platform = PlatformType.x64;
            Theme theme = new Theme(); //UI库

            //开启日志功能
            Logger.Instance.EnableInfoFile = true;

            //注册外部事件
            LoadFamily loadFamily = new LoadFamily();



        }
    }
}
