using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using QiShiLog;
using QiShiLog.Log;
using System.IO;

namespace FamilyManagerUI
{
    class LoadFamily : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            //【1】 获取当前文档
            Document doc = app.ActiveUIDocument.Document;
            UIDocument uIDocument = app.ActiveUIDocument;

            //【1】 先获取族路径
            string path = SysCache.Instance.CurrentRfaLocation; //从界面上获取的值
            string rfaName = Path.GetFileNameWithoutExtension(path); //族的名称
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            //【2】 载入到项目中
            Family family = new FilteredElementCollector(doc).OfClass(typeof(Family)).FirstOrDefault(x => x.Name == rfaName) as Family;
            if (family == null)
            {
                try
                {
                    Transaction transaction = new Transaction(doc, "载入族"); //事物
                    transaction.Start();
                    doc.LoadFamily(path, out family);//载入族
                    transaction.Commit();
                }
                catch (Exception ex)
                {

                    Logger.Instance.Info($"载入出错{doc},{ex}");
                    Process.Start(Path.Combine(QiShiCore.WorkSpace.Dir, "Log"));
                }
            }

            //【3】获取族类型
            FamilySymbol familySymbol = doc.GetElement(family.GetFamilySymbolIds().First<ElementId>()) as FamilySymbol;

            //【4】开始设置
            try
            {
                Logger.Instance.Info($"开始放置");
                uIDocument.PromptForFamilyInstancePlacement(familySymbol);

            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException e   )
            {

                Logger.Instance.Info($"用户取消了操作");

            }

            Process.Start(Path.Combine(QiShiCore.WorkSpace.Dir, "Log"));

        }

        public string GetName()
        {
            return "LoadFamily";
        }
    }
}
