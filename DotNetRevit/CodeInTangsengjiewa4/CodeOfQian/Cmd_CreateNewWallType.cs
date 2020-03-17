using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CodeInTangsengjiewa4.CodeOfQian
{
    /// <summary>
    /// WallType类型的Duplicate()方法创建一个墙类型,
    ///从FamilySymbol.Duplicate() 方法创建一个窗户类型。
    /// code from : https://blog.csdn.net/joexiongjin/article/details/6191299/
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class Cmd_CreateNewWallType : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Document doc = uiapp.ActiveUIDocument.Document;
            //get an existing dimension type.
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(DimensionType));
            DimensionType dimType = null;
            foreach (Element element in collector)
            {
                if (element.Name == "Linear Dimension Style")
                {
                    dimType = element as DimensionType;
                    break;
                }
            }
            DimensionType newType = dimType.Duplicate("Newtype") as DimensionType;
            if (newType != null)
            {
                Transaction ts = new Transaction(doc, "Excom");
                ts.Start();
                newType.get_Parameter(BuiltInParameter.LINE_PEN).Set(2);
                //you can change more here.
                doc.Regenerate();
                ts.Commit();
            }
            return Result.Succeeded;
        }
    }
}