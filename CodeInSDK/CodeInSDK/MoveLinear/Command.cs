using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CodeInSDK.MoveLinear
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Autodesk.Revit.UI.Result res = Result.Succeeded;
            Transaction trans = new Transaction(commandData.Application.ActiveUIDocument.Document, "MoveLinear");

            trans.Start();
            try
            {
                IEnumerator iter;
                Autodesk.Revit.UI.Selection.Selection sel;
                sel = commandData.Application.ActiveUIDocument.Selection;

                ElementSet elemSet;
                elemSet = new ElementSet();
                foreach (ElementId elementId in sel.GetElementIds())
                {
                    elemSet.Insert(commandData.Application.ActiveUIDocument.Document.GetElement(elementId));
                }

                //Check whether user has selected only one element
                if (0 == elemSet.Size)
                {
                    TaskDialog.Show("MoveLinear", "Please select an element");
                    trans.Commit();
                    return res;
                }
                if (1 < elemSet.Size)
                {
                    TaskDialog.Show("MoveLinear", "Please select only one element");
                    return res;
                }
                iter = elemSet.ForwardIterator();
                iter.MoveNext();

                Element element;

                element = (Element) iter.Current;

                if (element != null)
                {
                    LocationCurve lineLoc;
                    lineLoc = element.Location as LocationCurve;

                    if (null == lineLoc)
                    {
                        TaskDialog.Show("MoveLinear", "Please select an element which based on a line");
                        trans.Commit();
                        return res;
                    }

                    Line line;
                    //get start point via "get_EndPoint(0)";
                    XYZ newStart = new XYZ(lineLoc.Curve.GetEndPoint(0).X + 100,
                        lineLoc.Curve.GetEndPoint(0).Y,
                        lineLoc.Curve.GetEndPoint(0).Z);
                    //get end point via "get_EndPoint(1)";
                    XYZ newEnd = new XYZ(lineLoc.Curve.GetEndPoint(1).X,
                        lineLoc.Curve.GetEndPoint(1).Y + 100,
                        lineLoc.Curve.GetEndPoint(1).Z);

                    //get a new line and use it to move current element
                    //with property "Autodesk.Revit.DB.LocationCurve.Curve"
                    line = Line.CreateBound(newStart, newEnd);
                    lineLoc.Curve = line;
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("MoveLinear", ex.Message);
                res = Result.Failed;
            }
            finally
            {
            }
            trans.Commit();
            return res;
        }
    }
}