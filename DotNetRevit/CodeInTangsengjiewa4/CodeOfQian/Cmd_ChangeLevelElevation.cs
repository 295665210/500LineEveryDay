using System;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CodeInTangsengjiewa4.BinLibrary.Extensions;
using CodeInTangsengjiewa4.BinLibrary.Helpers;


namespace CodeInTangsengjiewa4.CodeOfQian
{
    /// <summary>
    /// create level
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class Cmd_ChangeLevelElevation : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            //根据标高值查找标高的名称
            doc.Invoke(m =>
            {
                Level level = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Levels)
                    .OfClass(typeof(Level)).Cast<Level>()
                    .FirstOrDefault(x => Math.Abs(x.Elevation - 8000d.MmToFeet()) < 1e-6);
                level.get_Parameter(BuiltInParameter.LEVEL_ELEV).Set(10000d.MmToFeet());
                level.get_Parameter(BuiltInParameter.DATUM_TEXT).Set("修改标高名称");
            }, "change level elevation value");
            return Result.Succeeded;
        }
    }
}