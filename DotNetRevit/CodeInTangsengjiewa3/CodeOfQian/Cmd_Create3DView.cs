using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CodeInTangsengjiewa3.CodeOfQian
{
    /// <summary>
    /// create 3d view
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class Cmd_Create3DView : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var app = uiapp.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;
            var sle = uidoc.Selection;

            Transaction ts = new Transaction(doc, "Create 3D view");
            ts.Start();

            //Find 3D  view type
            IEnumerable<ViewFamilyType> viewFamilyTypes =
                from ele in new FilteredElementCollector(doc).OfClass(typeof(ViewFamilyType))
                let type = ele as ViewFamilyType
                where type.ViewFamily == ViewFamily.ThreeDimensional
                select type;
            //create new perspective view3d
            //perspective: 透视
            View3D view3d = View3D.CreatePerspective(doc, viewFamilyTypes.First().Id);
            if (null != view3d)
            {
                //by default, the 3d view uses a default orientation
                //orientation: 方向
                XYZ eye = new XYZ(0, -100, 10);
                XYZ up = new XYZ(0, 0, 1);
                XYZ forward = new XYZ(0, 1, 0);
                view3d.SetOrientation(new ViewOrientation3D(eye, up, forward));
                //turn off the far clip plane with standard parameter ApI
                Parameter farClip = view3d.get_Parameter(BuiltInParameter.VIEWER_BOUND_ACTIVE_FAR);

                farClip.Set(1);
            }
            return Result.Succeeded;
        }
    }
}