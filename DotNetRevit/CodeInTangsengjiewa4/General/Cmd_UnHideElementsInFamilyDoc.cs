﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CodeInTangsengjiewa4.BinLibrary.Helpers;

namespace CodeInTangsengjiewa4.General
{
    /// <summary>
    /// show hided elements in family document of any view 
    /// </summary>
    class Cmd_UnHideElementsInFamilyDoc : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            if (!doc.IsFamilyDocument)
            {
                MessageBox.Show("请在租文档中使用该命令");
            }
            var views = doc.TCollector<View>().Where(m => !(m.IsTemplate));
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            var elementList = collector.WhereElementIsNotElementType();
            Transaction ts = new Transaction(doc, "显示族的隐藏元素");
            try
            {
                ts.Start();
                foreach (var view in views)
                {
                    if (view is ViewPlan || view is ViewSection || view is View3D)
                    {
                        foreach (var item in elementList)
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