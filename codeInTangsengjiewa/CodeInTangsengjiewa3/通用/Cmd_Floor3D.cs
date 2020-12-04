using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CodeInTangsengjiewa3.BinLibrary.Helpers;
using 唐僧解瓦.通用.UIs;

namespace CodeInTangsengjiewa3.通用
{
    /// <summary>
    /// 楼层三维
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_Floor3D : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            var acview = doc.ActiveView;

            var viewfamilytypes = doc.TCollector<ViewFamilyType>();
            var viewplanfamilytype = viewfamilytypes.Where(m => m.ViewFamily == ViewFamily.FloorPlan).First();
            var view3Dfamilytype = viewfamilytypes.Where(m => m.ViewFamily == ViewFamily.ThreeDimensional).First();

            var levels = doc.TCollector<Level>();

            FloorSelector fusi = FloorSelector.Instance;
            fusi.FloorBox.ItemsSource = levels;
            fusi.FloorBox.DisplayMemberPath = "Name";
            fusi.FloorBox.SelectedIndex = 0;

            var targetfloor = fusi.FloorBox.SelectionBoxItem as Level;
            var upperfloor = levels.Where(m => m.Elevation > targetfloor.Elevation)?.OrderBy(m => m.Elevation)
                ?.FirstOrDefault();

            var categories = doc.Settings.Categories;
            var modelcategories = categories.Cast<Category>().Where(m => m.CategoryType == CategoryType.Model).ToList();

            var filterlist = new List<ElementFilter>();
            foreach (var modelcategory in modelcategories)
            {
                var categoryfilter = new ElementCategoryFilter(modelcategory.Id);
                filterlist.Add(categoryfilter);
            }

            var logicalOrFilter = new LogicalOrFilter(filterlist);
            var collector = new FilteredElementCollector(doc);
            var modelelements = collector.WherePasses(logicalOrFilter).WhereElementIsNotElementType()
                .Where(m => m.Category.CategoryType == CategoryType.Model);

            var modelelementids = modelelements.Select(m => m.Id).ToList();

            var temboundingbox = default(BoundingBoxXYZ);

            Transaction temtran = new Transaction(doc, "temTransaction");
            temtran.Start();
            var temgroup = doc.Create.NewGroup(modelelementids);
            var temview = ViewPlan.Create(doc, viewplanfamilytype.Id, targetfloor.Id);
            temboundingbox = temgroup.get_BoundingBox(temview);
            temtran.RollBack();

            var zMin = targetfloor.Elevation;
            var zMax = upperfloor?.Elevation ?? temboundingbox.Max.Z;

            var oldmin = temboundingbox.Min;
            var oldmax = temboundingbox.Max;

            BoundingBoxXYZ newbox = new BoundingBoxXYZ();
            newbox.Min = new XYZ(oldmin.X, oldmin.Y, zMin);
            newbox.Max = new XYZ(oldmin.X, oldmax.Y, zMax);

            var new3dview = default(View3D);

            doc.Invoke(m => { new3dview = View3D.CreateIsometric(doc, view3Dfamilytype.Id); }, "楼层三维");
            new3dview.SetSectionBox(newbox);
            return Result.Succeeded;
        }
    }
}