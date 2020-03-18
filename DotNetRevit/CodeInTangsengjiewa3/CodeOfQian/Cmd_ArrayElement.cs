using System;
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

namespace CodeInTangsengjiewa3.CodeOfQian
{
    /// <summary>
    /// array element
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_ArrayElement : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Application app = uiapp.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;

            doc.Invoke(m =>
            {
                View view = doc.ActiveView;
                ElementId eleId = sel.PickObject(ObjectType.Element).ElementId;
                //表明阵列的方向
                XYZ translation = new XYZ(1000d.MmToFeet(), 2000d.MmToFeet(), 0);
                LinearArray.Create(doc, view, eleId, 3, translation, ArrayAnchorMember.Second);
                //count :陈列后的总数量
                //ArrayAnchorMember.Last: 相邻元素的间距为 将translation按count均分
                //ArrayAnchorMember.Second: 相邻元素的间距为 translation
            }, "阵列元素");
            return Result.Succeeded;
        }
    }
}