using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CodeInBuildingCoder1
{
    [Transaction(TransactionMode.Manual)]
    class CmdDuplicateElements : IExternalCommand
    {
        /// <summary>
        /// Create a new group of the specified elements.
        /// in the current active view at the given offset.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="ids"></param>
        /// <param name="offset"></param>
        void CreateGroup(Document doc, ICollection<ElementId> ids, XYZ offset)
        {
            Group group = doc.Create.NewGroup(ids);
            LocationPoint location = group.Location as LocationPoint;
            XYZ p = location.Point + offset;

            Group newGroup = doc.Create.PlaceGroup(p, group.GroupType);
            group.UngroupMembers();
        }

        public Result Execute(
            ExternalCommandData commandData, ref string message,
            ElementSet elements)
        {
            UIApplication app = commandData.Application;
            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = uidoc.Document;

            using (Transaction tx = new Transaction(doc))
            {
                tx.Start();
                Group group =
                    doc.Create.NewGroup(uidoc.Selection.GetElementIds());

                LocationPoint location = group.Location as LocationPoint;

                XYZ p = location.Point;
                XYZ newPoint = new XYZ(p.X, p.Y + 10, p.Z);
                Group newGroup =
                    doc.Create.PlaceGroup(newPoint, group.GroupType);
                group.UngroupMembers();

                ICollection<ElementId> eIds = newGroup.UngroupMembers();
                tx.Commit();
            }
            return Result.Succeeded;
        }
    }
}