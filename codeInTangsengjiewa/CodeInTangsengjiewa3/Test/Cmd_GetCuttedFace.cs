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

namespace CodeInTangsengjiewa3.Test
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_GetCuttedFace : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;

            var beam = sel.PickObject(ObjectType.Element).GetElement(doc) as FamilyInstance;
            var getcuttingelements = JoinGeometryUtils.GetJoinedElements(doc, beam).ToList();

            var beamsolid = beam.get_Geometry(new Options() {DetailLevel = ViewDetailLevel.Fine})
                .GetSolidOfGeometryObject();
            var othersolids = getcuttingelements.Select(m =>
                m.GetElement(doc).get_Geometry(new Options() {DetailLevel = ViewDetailLevel.Fine}));
            var resultsolid = default(Solid);

            foreach (var othersolid in othersolids)
            {
                //
            }

            return Result.Succeeded;
        }
    }
}