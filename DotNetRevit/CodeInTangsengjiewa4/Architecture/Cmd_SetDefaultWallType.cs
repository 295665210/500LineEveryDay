using System.Linq;
using System.Windows;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CodeInTangsengjiewa4.BinLibrary.Helpers;

namespace CodeInTangsengjiewa4.Architecture
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_SetDefaultWallType : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;
            var acview = uidoc.ActiveView;

            if (!(acview is ViewPlan))
            {
                MessageBox.Show("请在平面视图中运行此命令");
                return Result.Succeeded;
            }

            //获取目标墙
            var wallTypeCollector = new FilteredElementCollector(doc);
            var wallType = wallTypeCollector.OfClass(typeof(WallType)).Last();

            //显示墙类型名称
            MessageBox.Show(wallType.Name);

            //在事务中设置墙类型,并用设置好的墙类型创建墙
            doc.Invoke(m =>
            {
                doc.SetDefaultElementTypeId(ElementTypeGroup.WallType, wallType.Id);
                doc.Regenerate();
                Wall.Create(doc, Line.CreateBound(new XYZ(), new XYZ(100, 0, 0)), acview.GenLevel.Id, false);
            }, "create wall use the selected default wallType");
            return Result.Succeeded;
        }
    }
}