using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

using CodeInTangsengjiewa3.BinLibrary.Helpers;

namespace UnHideElementInFamily
{
    /// <summary>
    /// 显示族文件的各个视图里的隐藏元素
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_UnhideElementsInFamlyDoc : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            var acview = uidoc.ActiveView;

            if (!doc.IsFamilyDocument)
            {
                MessageBox.Show("这不是族文档,请在族文档中使用该命令");
            }

            var views = doc.TCollector<View>().Where(m => !(m.IsTemplate));

            FilteredElementCollector collector = new FilteredElementCollector(doc);

            var elelist = collector.WhereElementIsNotElementType();

            Transaction ts = new Transaction(doc, "显示族的隐藏元素");
            try
            {
                ts.Start();
                foreach (var view in views)
                {
                    if (view is ViewPlan || view is ViewSection || view is View3D)
                    {
                        foreach (var item in elelist)
                        {
                            if (item.IsHidden(view))
                            {
                                view.UnhideElements(new List<ElementId>() {item.Id});
                            }
                        }
                    }
                }
                ts.Commit();
            }
            catch (Exception e)
            {
                message = e.ToString();
                if (ts.GetStatus() == TransactionStatus.Started)
                {
                    ts.RollBack();
                }
            }
            return Result.Succeeded;
        }
    }
}