using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CodeInTangsengjiewa3.BinLibrary.Extensions;
using CodeInTangsengjiewa3.BinLibrary.Helpers;

namespace CodeInTangsengjiewa3.CodeOfQian
{
    /// <summary>
    /// copy element
    /// </summary>
    ///
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class Cmd_CopyElement : IExternalCommand
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
                // ICollection<Element> eles = new List<Element>();
                // eles.Add(ele);
                XYZ newTrans = new XYZ(1000d.MmToFeet(), 2000d.MmToFeet(), 0);
                var ele2 = ElementTransformUtils.CopyElement(doc, ele.Id, newTrans);
                string info = "";
                // int i = 0;
                info += "元素总个数为: ";
                TaskDialog.Show("tips", info);
            }, "copyelement");
            return Result.Succeeded;
        }
    }
}