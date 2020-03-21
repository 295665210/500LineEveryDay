using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CodeInTangsengjiewa4.BinLibrary.Helpers;
using CodeInTangsengjiewa4.General.UIs;

namespace CodeInTangsengjiewa4.General
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class Cmd_Floor3D : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;

            var viewFamilyTypes = doc.TCollector<ViewFamilyType>();
            var viewPlanFamilyType = viewFamilyTypes.First(m => m.ViewFamily == ViewFamily.FloorPlan);
            var view3DFamilyType = viewFamilyTypes.First(m => m.ViewFamily == ViewFamily.ThreeDimensional);

            var levels = doc.TCollector<Level>();
            FloorSelector floorSelectorUi = FloorSelector.Instance;
            floorSelectorUi.FloorBox.ItemsSource = levels;
            floorSelectorUi.FloorBox.DisplayMemberPath = "Name";
            floorSelectorUi.FloorBox.SelectedIndex = 0;
            floorSelectorUi.ShowDialog();

            var targetFloor = floorSelectorUi.FloorBox.SelectionBoxItem as Level;
            var upperFloor = levels.Where(m => m.Elevation > targetFloor.Elevation).OrderBy(m => m.Elevation).First();

            var categories = doc.Settings.Categories;
            var modelCategories = categories.Cast<Category>().Where(m => m.CategoryType == CategoryType.Model).ToList();
            var filtersList = new List<ElementFilter>();
            foreach (var modelCategory in modelCategories)
            {
                var categoryFilter = new ElementCategoryFilter(modelCategory.Id);
                filtersList.Add(categoryFilter);
            }
            var logicalOrFilter = new LogicalOrFilter(filtersList);
            var collector = new FilteredElementCollector(doc);
            var modelElements = collector.WherePasses(logicalOrFilter).WhereElementIsNotElementType()
                .Where(m => m.Category.CategoryType == CategoryType.Model);
            var modelElementIds = modelElements.Select(m => m.Id).ToList();
            var temBoundingBox = default(BoundingBoxXYZ);
            Transaction ts = new Transaction(doc, "temTransaction");
            ts.Start();
            var temGroup = doc.Create.NewGroup(modelElementIds);
            var temView = ViewPlan.Create(doc, viewPlanFamilyType.Id, targetFloor.Id);
            ts.RollBack();

            var zMin = targetFloor.Elevation;
            var zMax = upperFloor?.Elevation ?? temBoundingBox.Max.Z;

            var oldMin = temBoundingBox.Min;
            var oldMax = temBoundingBox.Max;

            BoundingBoxXYZ newBox = new BoundingBoxXYZ
            {
                Min = new XYZ(oldMin.X, oldMin.Y, zMin), Max = new XYZ(oldMax.X, oldMax.Y, zMax)
            };

            var new3dView = default(View3D);
            doc.Invoke(m =>
            {
                new3dView = View3D.CreateIsometric(doc, view3DFamilyType.Id);
                new3dView.SetSectionBox(newBox);
            }, "楼层三维");
            uidoc.ActiveView = new3dView;
            return Result.Succeeded;
        }
    }
}