﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

using View = Autodesk.Revit.DB.View;

namespace ExerciseProject
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class _0503ElementIsElementTypeFilter : IExternalCommand
    {
        /// <summary>
        ///使用FamilySymbolFilter
        /// 代码片段3-40
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            View acview = uidoc.ActiveView;
   


            Transaction ts = new Transaction(doc, "******");
            try
            {
                ts.Start();

                //找到所有属于ElementType的元素

                //创建收集器
                FilteredElementCollector collector = new FilteredElementCollector(doc);

                //创建过滤器
                ElementIsElementTypeFilter filter = new ElementIsElementTypeFilter();

                ICollection<ElementId> founds = collector.WherePasses(filter).ToElementIds();

                string info = null;
                info += "找到" + founds.Count + "个ElementType";

                TaskDialog.Show("提示", info);

                ts.Commit();
            }
            catch (Exception)
            {
                if (ts.GetStatus() == TransactionStatus.Started)
                {
                    ts.RollBack();
                }
            }

            return Result.Succeeded;
        }
    }
}