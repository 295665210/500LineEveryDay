using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CodeInTangsengjiewa3.BinLibrary.Extensions;

namespace CodeInTangsengjiewa3.Test
{
    /// <summary>
    /// 测试楼板顶面数量
    /// </summary>
    class Cmd_SurfaceTest : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var ap = uiapp.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;

            var sel = uidoc.Selection;
            var acview = uidoc.ActiveView;

            var floor =
                sel.PickObject(ObjectType.Element, doc.GetSelectionFilter(m => m is Floor))
                    .GetElement(doc) as Floor;
            var faces = HostObjectUtils.GetTopFaces(floor);
            MessageBox.Show(faces.Count.ToString());
            return Result.Succeeded;
        }
    }
}