using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace DMUDemo
{
    class DMUUpdater : IUpdater
    {
        internal static AddInId m_appId = new AddInId(new Guid("8f7347e8-f1f0-43b4-8012-94d6ab46f30d")); //addin文件的ID
        internal static UpdaterId
            m_updaterId = new UpdaterId(m_appId, new Guid("120279DF-0023-4D07-857A-80357933F777")); //DMU自己的ID，新建一个。

        private WallType m_wallType = null;

        //更新的内容，在 data 里面。
        public void Execute(UpdaterData data)
        {
            //如果不开启动态更新，则不执行
            if (!SysCache.Instance.IsEnable)
            {
                return;
            }

            //获取当前发生改变的文档
            Document doc = data.GetDocument();

            if (m_wallType == null)
            {
                m_wallType =
                    new FilteredElementCollector(doc).OfClass(typeof(WallType)).First(x => x.Name == "幕墙") as WallType;
            }

            //将新增的元素改为幕墙类型
            foreach (ElementId elementId in data.GetModifiedElementIds())
            {
                Wall wall = doc.GetElement(elementId) as Wall;
                TaskDialog.Show("提示", $"{wall.Name}说： hi，我体积变了  ");
            }
        }

        //得到更新的ID
        public UpdaterId GetUpdaterId() => m_updaterId;

        //优先搜索的对象，提高搜索效率
        public ChangePriority GetChangePriority()
        {
            return ChangePriority.FloorsRoofsStructuralWalls;
        }

        //得到更新器的名字
        public string GetUpdaterName()
        {
            return "墙更新";
        }

        //插件叫什么名字，以及对应的提示信息
        public string GetAdditionalInformation()
        {
            return "墙DMU更新";
        }
    }
}