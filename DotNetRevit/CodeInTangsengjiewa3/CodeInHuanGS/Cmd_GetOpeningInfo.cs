using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CodeInTangsengjiewa3.BinLibrary.Extensions;

namespace CodeInTangsengjiewa3.CodeInHuanGS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_GetOpeningInfo : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var app = uiapp.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;

            Opening ele =
                sel.PickObject(ObjectType.Element, doc.GetSelectionFilter(m => m is Opening))
                    .GetElement(doc) as Opening;
            if (ele.Category.Id.IntegerValue == (int) BuiltInCategory.OST_ShaftOpening)
            {
                TaskDialog.Show("tips", "竖井洞口,主体为null");
                return Result.Cancelled;
            }
            else
            {
                GetInfoOpening(ele);
                return Result.Succeeded;
            }
        }

        private void GetInfoOpening(Opening opening)
        {
            string message = "Open : \n";
            message += "\n The id of the opening's host element is :" + opening.Host.Id.IntegerValue;
            if (opening.IsRectBoundary)
            {
                message += "\n The opening has a rectangle boundary.";
                IList<XYZ> boundaryRect = opening.BoundaryRect;
                XYZ point = boundaryRect[0];
                message += "\n Min Coordinate point :" + XyzToString(point);

                point = boundaryRect[1];
                message += "\n Max coordinate Point :" + XyzToString(point);
            }
            else
            {
                message += "\n The opening does not have a rectangle boundary";
                int curves = opening.BoundaryCurves.Size;
                message += "\n Number of curves is : " + curves;
                for (int i = 0; i < curves; i++)
                {
                    Curve curve = opening.BoundaryCurves.get_Item(i);
                    message += "\n Curve start point :" + XyzToString(curve.GetEndPoint(0));
                    message += "\n Curve end point :" + XyzToString(curve.GetEndPoint(1));
                }
            }
            TaskDialog.Show("info of opening", message);
        }

        public string XyzToString(XYZ point)
        {
            return "(" + point.X.ToString("F") + point.Y.ToString("F") + point.Z.ToString("F") + ")";
        }
    }
}