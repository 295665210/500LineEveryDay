using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace CodeInTangsengjiewa4.CodeInHuanGS
{
    /// <summary>
    /// Revit 设置模型线的颜色有两种方法:
    /// 方法一: 新建线的样式,设置
    /// 方法二: 替换视图中的图形,只对当前视图有效.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_ModelLineColorOverRide : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Application app = uiapp.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            View view = doc.ActiveView; //只有get, 推测是一个封装的属性
            // var views = uidoc.ActiveView;  //有set 和get
            Reference reference = uidoc.Selection.PickObject(ObjectType.Element);
            Element element = doc.GetElement(reference);

            //方法二:
            OverrideGraphicSettings ogs = view.GetElementOverrides(element.Id);
            Transaction ts = new Transaction(doc, "替换视图中的图形");
            ts.Start();
            ogs.SetProjectionLineColor(new Color(0, 255, 0));
            view.SetElementOverrides(element.Id, ogs);
            ts.Commit();
            return Result.Succeeded;
        }
    }
}