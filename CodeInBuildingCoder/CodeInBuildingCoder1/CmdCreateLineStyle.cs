using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CodeInBuildingCoder1
{
    [Transaction(TransactionMode.Manual)]
    class CmdCreateLineStyle : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData, ref string message,
            ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            CreateLineStyle(doc);
            return Result.Succeeded;
        }

        /// <summary>
        /// Create a new line style using New SubCategory
        /// </summary>
        /// <param name="doc"></param>
        void CreateLineStyle(Document doc)
        {
            FilteredElementCollector fec =
                new FilteredElementCollector(doc)
                    .OfClass(typeof(LinePatternElement));

            LinePatternElement lingPatternElement = fec
                .Cast<LinePatternElement>()
                .First<LinePatternElement>(linePattern =>
                                               linePattern.Name ==
                                               "Long Dash");
            //The new lineStyle will be a subcategory
            //of the Lines Category
            Categories categories = doc.Settings.Categories;

            Category lineCat =
                categories.get_Item(BuiltInCategory.OST_Lines);

            using (Transaction tx = new Transaction(doc))
            {
                tx.Start();
                Category newLineStyleCat =
                    categories.NewSubcategory(lineCat, "New LineStyle");
                doc.Regenerate();
                newLineStyleCat
                    .SetLineWeight(8, GraphicsStyleType.Projection);
                newLineStyleCat.LineColor = new Color(0xFF, 0x00, 0x00);
                newLineStyleCat.SetLinePatternId(lingPatternElement.Id,
                                                 GraphicsStyleType
                                                     .Projection);
                tx.Commit();
            }
        }
    }
}