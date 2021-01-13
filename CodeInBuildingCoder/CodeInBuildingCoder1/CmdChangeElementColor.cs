using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Application = Autodesk.Revit.ApplicationServices.Application;
using Document = Autodesk.Revit.DB.Document;

namespace CodeInBuildingCoder1
{
    [Transaction(TransactionMode.Manual)]
    class CmdChangeElementColor : IExternalCommand
    {
        void ChangeElementColor(Document doc, ElementId id)
        {
            Color color = new Color((byte) 200, (byte) 100, (byte) 100);
            OverrideGraphicSettings ogs = new OverrideGraphicSettings();
            ogs.SetProjectionLineColor(color);

            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Change Element Color");
                doc.ActiveView.SetElementOverrides(id, ogs);
                tx.Commit();
            }
        }

        void ChangeElementMaterial(Document doc, ElementId id)
        {
            Element e = doc.GetElement(id);
            if (null != e.Category)
            {
                int im = e.Category.Material.Id.IntegerValue;

                List<Material> materials =
                    new List<Material>(new FilteredElementCollector(doc).WhereElementIsNotElementType()
                                           .OfClass(typeof(Material)).ToElements()
                                           .Where<Element>(m => m.Id.IntegerValue != im).Cast<Material>());
                string s = "begein";
                foreach (string s1 in materials.Select(m => m.Name))
                {
                    s = s + "\r\n " + s1;
                }
                TaskDialog.Show("material", s);

                Random r = new Random();
                int i = r.Next(materials.Count);

                using (Transaction tx = new Transaction(doc))
                {
                    tx.Start("Change Element Material");
                    e.Category.Material = materials[i];
                    tx.Commit();
                }
            }
        }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Application app = uiapp.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            View view = doc.ActiveView;
            ElementId id;
            try
            {
                Selection sel = uidoc.Selection;
                Reference r = sel.PickObject(ObjectType.Element, "Pick element to change its color.");
                id = r.ElementId;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled;
            }

            ChangeElementColor(doc, id);
            ChangeElementMaterial(doc, id);
            uidoc.RefreshActiveView();
            return Result.Succeeded;
        }
    }
}