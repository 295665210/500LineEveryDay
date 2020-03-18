using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CodeInTangsengjiewa3.BinLibrary.Extensions;
using CodeInTangsengjiewa3.BinLibrary.Helpers;

namespace CodeInTangsengjiewa3.CodeOfQian
{
    /// <summary>
    /// mirror wall
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_MirrorElement : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var app = uiapp.Application;
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

        public void MirrorWall(Document doc, Wall wall)
        {
            Reference reference = HostObjectUtils.GetSideFaces(wall, ShellLayerType.Exterior).First();
            Face face = wall.GetGeometryObjectFromReference(reference) as Face;
            UV boxMin = face.GetBoundingBox().Min;
            Plane plane = Plane.CreateByNormalAndOrigin(face.ComputeNormal(boxMin),
                                                        face.Evaluate(boxMin).Add(new XYZ(10, 10, 0)));
            ElementTransformUtils.MirrorElement(doc, wall.Id, plane);
        }
    }
}