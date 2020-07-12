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

namespace RevitFoundation.CodeInQuick
{
    [Transaction(TransactionMode.Manual)]
    internal class DisplaySomething : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Application app = uiapp.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document document = uiapp.ActiveUIDocument.Document;

            View activeView = uidoc.ActiveView;

            Result result;

            try
            {
                for (;;)
                {
                    Selection selection = uidoc.Selection;
                    Reference reference = selection.PickObject(ObjectType.PointOnElement, "请选择图层");
                    Element element = document.GetElement(reference);
                    GeometryObject geometryObjectFromReference = element.GetGeometryObjectFromReference(reference);
                    Category category = null;
                    if (geometryObjectFromReference.GraphicsStyleId != ElementId.InvalidElementId)
                    {
                        GraphicsStyle graphicsStyle =
                            document.GetElement(geometryObjectFromReference.GraphicsStyleId) as GraphicsStyle;
                        if (graphicsStyle != null)
                        {
                            category = graphicsStyle.GraphicsStyleCategory;
                        }
                    }
                    Transaction transaction = new Transaction(document, "隐藏图层");
                    transaction.Start();
                    if (category != null)
                    {
                        document.ActiveView.SetCategoryHidden(category.Id, true);
                    }
                    transaction.Commit();
                }
            }
            catch (Exception)
            {
                result = Result.Succeeded;
            }

            return result;
        }
    }
}