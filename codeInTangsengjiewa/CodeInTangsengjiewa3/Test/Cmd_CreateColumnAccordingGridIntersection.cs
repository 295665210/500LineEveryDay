using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using CodeInTangsengjiewa3.BinLibrary.Helpers;
using CodeInTangsengjiewa3.BinLibrary.RevitHelper;
using 唐僧解瓦.Test.UIs;

namespace CodeInTangsengjiewa3.Test
{
    /// <summary>
    ///在轴线交点出生成柱子
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    [Regeneration(RegenerationOption.Manual)]
    class Cmd_CreateColumnAccordingGridIntersection : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;
            var acview = uidoc.ActiveView;

            //filter target columnTypes
            ElementFilter architectureColumnFilter = new ElementCategoryFilter(BuiltInCategory.OST_Columns);
            ElementFilter structralColumnFilter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralColumns);
            ElementFilter ofFilter = new LogicalOrFilter(architectureColumnFilter, structralColumnFilter);
            var collector = new FilteredElementCollector(doc);
            var columntypes = collector.WhereElementIsElementType().WherePasses(ofFilter).ToElements();

            ColumnTypesForm typesForm = ColumnTypesForm.Getinstance(columntypes.ToList());
            typesForm.ShowDialog(RevitWindowHelper.GetRevitWindow());

            var familysymbol = typesForm.symbolCombo.SelectedItem as FamilySymbol;

            var bottomlevel = default(Level);
            var bottomoffset = default(double);

            var toplevel = default(Level);
            var topoffset = default(double);

            var grids = doc.TCollector<Grid>();
            var points = new List<XYZ>();
            foreach (var grid in grids)
            {
                foreach (var grid1 in grids)
                {
                    if (grid.Id == grid1.Id)
                    {
                        continue;
                    }

                    var curve1 = grid.Curve;
                    var curve2 = grid1.Curve;
                    var res = new IntersectionResultArray();
                    var intersecRes = curve1.Intersect(curve2, out res);
                    if (intersecRes != SetComparisonResult.Disjoint)
                    {
                        if (res != null)
                        {
                            points.Add(res.get_Item(0).XYZPoint);
                        }
                    }
                }
            }
            //distinct points on same location
            points = points.Where((m, i) => points.FindIndex(n => n.IsAlmostEqualTo(m)) == i).ToList();

            //createColumns as intersection point
            TransactionGroup tsg = new TransactionGroup(doc);
            tsg.Start("统一创建柱子");
            foreach (var point in points)
            {
                doc.Invoke(m =>
                {
                    if (!familysymbol.IsActive)
                    {
                        familysymbol.Activate();
                    }
                    var instance = doc.Create.NewFamilyInstance(point, familysymbol, acview.GenLevel,
                        StructuralType.NonStructural);
                }, "创建柱子");
            }
            tsg.Assimilate();
            return Result.Succeeded;
        }
    }
}