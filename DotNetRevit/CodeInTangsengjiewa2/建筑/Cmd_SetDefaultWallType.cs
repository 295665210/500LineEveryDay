﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CodeInTangsengjiewa2.建筑
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class Cmd_SetDefaultWallType : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = commandData.Application.ActiveUIDocument;
            var doc = uidoc.Document;
            var acview = uidoc.ActiveView;
            var sel = uidoc.Selection;

            if (!(acview is ViewPlan))
            {
                MessageBox.Show("请在平面视图中运行此命令");
                return Result.Cancelled;
            }
            // 获取目标墙
            var walltypeCollector = new FilteredElementCollector(doc);
            var walltype = walltypeCollector.OfClass(typeof(WallType)).Last();

            //显示墙类型名称
            MessageBox.Show(walltype.Name);

            //在事务中设置墙类型,并用设置好的类型创建墙
            Transaction ts = new Transaction(doc, "设置墙类型,并窗间墙");
            ts.Start();
            doc.SetDefaultElementTypeId(ElementTypeGroup.WallType, walltype.Id);
            doc.Regenerate();

            //创建墙
            Wall.Create(doc, Line.CreateBound(new XYZ(), new XYZ(100, 0, 0)), acview.GenLevel.Id, false);
            ts.Commit();

            return Result.Succeeded;
        }
    }
}