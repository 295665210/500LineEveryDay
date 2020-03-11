﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CodeInTangsengjiewa3.BinLibrary.Extensions;
using CodeInTangsengjiewa3.BinLibrary.Helpers;

namespace CodeInTangsengjiewa3.通用
{
    /// <summary>
    /// 框选元素,形成三维框
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_PickBox3D : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Application app = uiapp.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            View acview = uidoc.ActiveView;

            var viewfamilytype =
                doc.TCollector<ViewFamilyType>().First(m => m.ViewFamily == ViewFamily.ThreeDimensional);

            var elementRefs = sel.PickObjects(ObjectType.Element,
                                              doc.GetSelectionFilter(m =>
                                              {
                                                  return m.Category.CategoryType ==
                                                         CategoryType.Model;
                                              })); ///?????????????????????????
            var eles = elementRefs.Select(m => m.ElementId.GetElement(doc));
            var eleids = elementRefs.Select(m => m.ElementId).ToList();
            var tembox = default(BoundingBoxXYZ);
            Transaction temtran = new Transaction(doc, "temTran");
            temtran.Start();
            var group = doc.Create.NewGroup(eleids);
            tembox = group.get_BoundingBox(acview);
            temtran.RollBack();

            var newAcview = default(View);
            doc.Invoke(m =>
            {
                var _3dview = View3D.CreateIsometric(doc, viewfamilytype.Id);
                _3dview.SetSectionBox(tembox);
                newAcview = _3dview;
            }, "框选三维");
            uidoc.ActiveView = newAcview;

            return Result.Succeeded;
        }
    }
}