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

namespace DMUDemo
{
    /// <summary>
    /// 就是 Application,管理整个事件。
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class DMUManager : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            UpdaterRegistry.UnregisterUpdater(DMUUpdater.m_updaterId);
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            SysCache.Instance.IsEnable = false;
            //【1】 注册模型动态更新
            DMUUpdater dmuUpdater = new DMUUpdater();
            UpdaterRegistry.RegisterUpdater(dmuUpdater);

            //【2】设置触发条件
            //【2-1】监视对象
            ElementClassFilter wallFilter = new ElementClassFilter(typeof(Wall));
            //【2-2】对象变化情况（可以一个，或者多个）
            // ChangeType geometryChange = Element.GetChangeTypeGeometry(); //几何变化
            // ChangeType countChange = Element.GetChangeTypeElementAddition();//个数变化

            ChangeType changeTypes = ChangeType.ConcatenateChangeTypes(Element.GetChangeTypeGeometry(),
                Element.GetChangeTypeElementAddition());
            //【3】添加触发器
            UpdaterRegistry.AddTrigger(dmuUpdater.GetUpdaterId(), wallFilter, changeTypes);
            return Result.Succeeded;
        }
    }

    /// <summary>
    /// 打开DMU
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class DMUTurnOn : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            SysCache.Instance.IsEnable = true;
            return Result.Succeeded;
        }
    }

    /// <summary>
    /// 关闭DMU
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class DMUTurnOff : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            SysCache.Instance.IsEnable = false;
            return Result.Succeeded;
        }
    }
}