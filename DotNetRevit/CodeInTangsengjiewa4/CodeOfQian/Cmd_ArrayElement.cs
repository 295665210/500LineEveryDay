using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CodeInTangsengjiewa4.BinLibrary.Extensions;
using CodeInTangsengjiewa4.BinLibrary.Helpers;


namespace CodeInTangsengjiewa4.CodeOfQian
{
    /// <summary>
    /// array element
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_ArrayElement : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;

            doc.Invoke(m =>
            {
                View view = doc.ActiveView;
                ElementId elementId = sel.PickObject(ObjectType.Element).ElementId;
                //表明阵列的方向
                XYZ translation = new XYZ(1000d.MmToFeet(), 2000d.MmToFeet(), 0);
                LinearArray.Create(doc, view, elementId, 3, translation, ArrayAnchorMember.Second);
                //Count: 阵列后的总数量
                //ArrayAnchorMember.Last :相邻元素的间距为 将translation按count均分;
                //ArrayAnchorMember.Second: 相邻元素的间距为translation
            }, "array element");
            return Result.Succeeded;
        }
    }
}