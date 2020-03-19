using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CodeInTangsengjiewa4.BinLibrary.Extensions;
using CodeInTangsengjiewa4.BinLibrary.Helpers;

namespace CodeInTangsengjiewa4.CodeOfQian
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_MirrorElement : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;

            doc.Invoke(m =>
            {
                Wall wall =
                    sel.PickObject(ObjectType.Element, doc.GetSelectionFilter(x => x is Wall)).GetElement(doc) as Wall;
                MirrorWall(doc, wall);
            }, "mirror element");
            return Result.Succeeded;
        }

        private void MirrorWall(Document doc, Wall wall)
            //private 同一个class struct中能被访问,也是class 和struct 中的默认访问修饰符

        {
            Reference reference = HostObjectUtils.GetSideFaces(wall, ShellLayerType.Exterior).First();
            Face face = wall.GetGeometryObjectFromReference(reference) as Face;
            UV boxMin = face.GetBoundingBox().Min;
            Plane plane =
                Plane.CreateByNormalAndOrigin(face.ComputeNormal(boxMin),
                                              face.Evaluate(boxMin).Add(new XYZ(10, 10, 0)));
            ElementTransformUtils.MirrorElement(doc, wall.Id, plane);
        }
    }
}