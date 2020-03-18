using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CodeInTangsengjiewa4.BinLibrary.Extensions;
using CodeInTangsengjiewa4.BinLibrary.Helpers;


namespace CodeInTangsengjiewa4.CodeOfQian
{
    /// <summary>
    /// copy element
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_CopyElement : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Application app = uiapp.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;

            doc.Invoke(m =>
            {
                Element ele = sel.PickObject(ObjectType.Element, "请选择一个元素").GetElement(doc);
                XYZ newTrans = new XYZ(1000d.MmToFeet(), 2000d.MmToFeet(), 0);
                var element2 = ElementTransformUtils.CopyElement(doc, ele.Id, newTrans);
                string info = "";
                info += "元素的总个数为 :";
                TaskDialog.Show("tips", info);
            }, "copy element");
            return Result.Succeeded;
        }
    }
}