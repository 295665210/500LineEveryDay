using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CodeInTangsengjiewa4.BinLibrary.Extensions;
using System.Collections.Generic;

namespace CodeInTangsengjiewa4.CodeInHuanGS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_GetOpeningInfo : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Application app = uiapp.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;

            Opening ele =
                sel.PickObject(ObjectType.Element, doc.GetSelectionFilter(m => m is Opening))
                    .GetElement(doc) as Opening;
            if (ele.Category.Id.IntegerValue == (int) BuiltInCategory.OST_ShaftOpening)
            {
                TaskDialog.Show("tips", "竖井洞口,主体为 null");
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
                message += "\n Min coordinate point :" + XyzToString(point);
                point = boundaryRect[1];
                message += "\n Max coordinate point :" + XyzToString(point);
            }
            else
            {
                message += "\n The opening does not have a rectangle boundary";
                int curvesSize = opening.BoundaryCurves.Size;
                message += "\n Number of curve is :" + curvesSize;
                for (int i = 0; i < curvesSize; i++)
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