﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CodeInTangsengjiewa2.BinLibrary.Extensions;

namespace CodeInTangsengjiewa2.通用
{
    /// <summary>
    /// 隐藏族文档中的元素
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_HideElementInfamilyDoc : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            View acview = uidoc.ActiveView;

            var ele = sel.PickObject(ObjectType.Element);
            Transaction ts = new Transaction(doc, "****");
            try
            {
                ts.Start();
                acview.HideElements(new Collection<ElementId>() {ele.ElementId});
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