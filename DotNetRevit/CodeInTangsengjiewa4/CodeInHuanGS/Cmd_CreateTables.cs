using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CodeInTangsengjiewa4.BinLibrary.Extensions;
using System;
using System.Linq;


namespace CodeInTangsengjiewa4.CodeInHuanGS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_CreateTables : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Application app = commandData.Application.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;

            Transaction ts = new Transaction(doc, "将族下的每个symbol创建实例");
            ts.Start();
            //try to load family
            string fileName = @"C:\ProgramData\Autodesk\RVT 2020\Libraries\China\建筑\家具\3D\桌椅\桌椅组合\餐桌 - 圆形带餐椅.rfa";
            Family family = null;
            if (!doc.LoadFamily(fileName, out family))
            {
                throw new Exception("Unable to load" + fileName);
            }
            //loop throw table symbols and add a new table for each
            var symbolItor = family.GetFamilySymbolIds().Select(m => m.GetElement(doc)).GetEnumerator();
            double x = 0, y = 0;
            while (symbolItor.MoveNext())
            {
                FamilySymbol symbol = symbolItor.Current as FamilySymbol;
                symbol.Activate();
                XYZ location = new XYZ(x, y, 10);
                FamilyInstance instance = doc.Create.NewFamilyInstance(location, symbol, StructuralType.NonStructural);
                x += 10;
            }
            ts.Commit();
            return Result.Succeeded;
        }
    }
}