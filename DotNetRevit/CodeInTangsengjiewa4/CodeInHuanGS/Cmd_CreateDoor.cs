using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CodeInTangsengjiewa4.BinLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeInTangsengjiewa4.CodeInHuanGS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_CreateDoor : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var app = uiapp.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;

            Transaction ts = new Transaction(doc, "create door");
            ts.Start();
            Wall wall =
                sel.PickObject(ObjectType.Element, doc.GetSelectionFilter(m => m is Wall)).GetElement(doc) as Wall;
            CreateDoorsInWall(doc, wall);
            ts.Commit();
            uidoc.RefreshActiveView();
            return Result.Succeeded;
        }

        private void CreateDoorsInWall(Document doc, Wall wall)
        {
            string fileName = @"C:\ProgramData\Autodesk\RVT 2020\Libraries\China\消防\建筑\防火门\单扇防火门.rfa";
            Family family = null;
            if (!doc.LoadFamily(fileName, out family))
            {
                throw new Exception("unable to load" + fileName);
            }
            Level level = doc.ActiveView.GenLevel;
            IEnumerator<Element> symbolEnumerator =
                family.GetFamilySymbolIds().Select(m => m.GetElement(doc)).GetEnumerator();
            double x = 0, y = 0, z = 0;
            while (symbolEnumerator.MoveNext())
            {
                FamilySymbol symbol = symbolEnumerator.Current as FamilySymbol;
                symbol.Activate();
                XYZ location = new XYZ(x, y, z);
                FamilyInstance instance =
                    doc.Create.NewFamilyInstance(location, symbol, wall, level, StructuralType.NonStructural);
                x += 2000d.MmToFeet();
                y += 1000d.MmToFeet();
                z += 500d.MmToFeet();
            }
        }
    }
}