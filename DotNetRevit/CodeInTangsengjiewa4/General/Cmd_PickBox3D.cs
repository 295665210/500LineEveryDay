using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CodeInTangsengjiewa4.BinLibrary.Extensions;
using CodeInTangsengjiewa4.BinLibrary.Helpers;

namespace CodeInTangsengjiewa4.General
{
    /// <summary>
    /// 框选元素,形成三维框
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_PickBox3D : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            View acview = uidoc.ActiveView;

            var viewFamilyType =
                doc.TCollector<ViewFamilyType>().First(m => m.ViewFamily == ViewFamily.ThreeDimensional);
            var elementRefs = sel.PickObjects(ObjectType.Element, doc.GetSelectionFilter(m => m.Category.CategoryType ==
                                                                                              CategoryType.Model));

            var eles = elementRefs.Select(m => m.ElementId.GetElement(doc));
            var elementIds = elementRefs.Select(m => m.ElementId).ToList();
            var temBox = default(BoundingBoxXYZ);
            Transaction temTran = new Transaction(doc, "temTran");
            temTran.Start();
            var group = doc.Create.NewGroup(elementIds);
            temBox = group.get_BoundingBox(acview);
            temTran.RollBack();

            var newAcview = default(View);
            doc.Invoke(m =>
            {
                var _3dView = View3D.CreateIsometric(doc, viewFamilyType.Id);
                _3dView.SetSectionBox(temBox);
                newAcview = _3dView;
            }, "框选三维");
            uidoc.ActiveView = newAcview;
            return Result.Succeeded;
        }
    }
}