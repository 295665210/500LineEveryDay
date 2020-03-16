using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CodeInTangsengjiewa4.BinLibrary.Extensions;
using System.Collections.Generic;

namespace CodeInTangsengjiewa4.CodeInHuanGS
{
    [Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_GetSubAndSuperComponents : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Application app = uiapp.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;

            FamilyInstance f =
                sel.PickObject(ObjectType.Element, doc.GetSelectionFilter(m => m is FamilyInstance))
                    .GetElement(doc) as FamilyInstance;
            GetSubAndSuperComponents(f);
            return Result.Succeeded;
        }

        public void GetSubAndSuperComponents(FamilyInstance familyInstance)
        {
            ICollection<ElementId> subEleSet = familyInstance.GetSubComponentIds();
            if (subEleSet != null)
            {
                string subElementNames = "\n";
                foreach (ElementId id in subEleSet)
                {
                    FamilyInstance f = familyInstance.Document.GetElement(id) as FamilyInstance;
                    subElementNames += f.Name + "\n";
                }
                string info = "SubComponent count = " + subEleSet.Count;
                info += "\n" + subElementNames;
                TaskDialog.Show("SubElement", info);
            }
            if (familyInstance.SuperComponent is FamilyInstance super)
            {
                TaskDialog.Show("SuperComponent", "SuperComponent :" + super.Name);
            }
        }
    }
}