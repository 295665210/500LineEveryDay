using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using View = Autodesk.Revit.DB.View;

namespace DimensionLeaderEnd
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]

    //调整尺寸标注文字的位置 ：
    //MoveHorizontally 标注的文字水平移动；
    //MoveToPickedPoint:标注的文字水平移动到鼠标点击的位置
    public class MoveHorizontally : IExternalCommand
    {
        private double m_delta = -10;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //get the handle of current document.
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            using (Transaction ts = new Transaction(doc))
            {
                //get the element selection of current document.
                Selection selection = uidoc.Selection;
                ICollection<ElementId> selectedIds = uidoc.Selection.GetElementIds();

                if (0 == selectedIds.Count)
                {
                    //if no elements selected.
                    TaskDialog.Show("Revit", "You haven't selected any elements.");
                }
                else
                {
                    foreach (ElementId id in selectedIds)
                    {
                        Dimension dim = doc.GetElement(id) as Dimension;
                        if (null != dim)
                        {
                            Line dimLine = dim.Curve as Line;
                            if (dimLine != null)
                            {
                                ts.Start("Set leader end position.");
                                try
                                {
                                    XYZ dir = dimLine.Direction;
                                    if (dim.Segments.IsEmpty)
                                    {
                                        XYZ leaderPos = ComputeLeaderPosition(dir, dim.Origin);
                                        dim.LeaderEndPosition = leaderPos;
                                    }
                                    else
                                    {
                                        foreach (DimensionSegment ds in dim.Segments)
                                        {
                                            XYZ leaderPos = ComputeLeaderPosition(dir, ds.Origin);
                                            ds.LeaderEndPosition = leaderPos;
                                        }
                                    }
                                    ts.Commit();
                                }
                                catch (Exception ex)
                                {
                                    TaskDialog.Show("Can't set dimension leader end point:{0}", ex.Message);
                                }
                            }
                        }
                    }
                }
                return Result.Succeeded;
            }
        }

        private XYZ ComputeLeaderPosition(XYZ dir, XYZ origin)
        {
            XYZ leaderPos = new XYZ();
            leaderPos = dir * m_delta;
            leaderPos = leaderPos.Add(origin);
            return leaderPos;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class MoveToPickedPoint : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //get the handle of current document.
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            using (Transaction _transaction = new Transaction(doc))
            {
                // get the element selection of current document.
                Selection selection = uidoc.Selection;
                ICollection<ElementId> selectedIds = uidoc.Selection.GetElementIds();
                if (0 == selectedIds.Count)
                {
                    //if no elements selected.
                    TaskDialog.Show("Revit", "You haven't selected any elements");
                }
                else
                {
                    foreach (ElementId id in selectedIds)
                    {
                        Dimension dim = doc.GetElement(id) as Dimension;
                        if (dim != null)
                        {
                            XYZ startPoint = selection.PickPoint(ObjectSnapTypes.None, "PickStart");
                            _transaction.Start("Set leader end point");
                            try
                            {
                                if (dim.Segments.IsEmpty)
                                {
                                    dim.LeaderEndPosition = startPoint;
                                }
                                else
                                {
                                    XYZ deltaVec = dim.Segments.get_Item(1).Origin
                                        .Subtract(dim.Segments.get_Item(0).Origin);
                                    XYZ offset = new XYZ();
                                    foreach (DimensionSegment ds in dim.Segments)
                                    {
                                        ds.LeaderEndPosition = startPoint.Add(offset);
                                        offset = offset.Add(deltaVec);
                                    }
                                }
                                _transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                TaskDialog.Show("Canot set dimension leader end point:{0}", ex.Message);
                                _transaction.RollBack();
                            }
                        }
                    }
                }
            }
            return Result.Succeeded;
        }
    }
}