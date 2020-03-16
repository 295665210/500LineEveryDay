using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CodeInTangsengjiewa.BinLibrary.Extensions;
using CodeInTangsengjiewa.BinLibrary.Helpers;
using System.Collections.Generic;

namespace CodeInTangsengjiewa4.CodeInHuanGS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_GetFaces : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Application app = uiapp.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;

            Reference reference = sel.PickObject(ObjectType.Element, "选个东西");
            var elementId = reference.ElementId;
            Options opt = new Options();
            opt.DetailLevel = ViewDetailLevel.Fine;
            GeometryElement geoElem = doc.GetElement(reference).get_Geometry(opt);
            List<Face> faces = geoElem.GetFaces();
            doc.Invoke(m =>
            {
                ElementId yellowPaintId = new ElementId(12859);
                doc.Paint(elementId, faces[0], yellowPaintId);
            }, "paint");
            return Result.Succeeded;
        }
    }
}