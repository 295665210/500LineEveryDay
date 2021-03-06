﻿using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using View = Autodesk.Revit.DB.View;

namespace RevitDevelopmentFoundation.Chapter04
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class R0415CreateDoor : IExternalCommand
    {
        /// <summary>
        /// 代码片段4-15 墙上创建门
        /// </summary>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            View acView = uidoc.ActiveView;

            Transaction ts = new Transaction(doc, "******");

            try
            {
                ts.Start();

                FamilySymbol faiFamilySymbol = doc.GetElement(new ElementId(341531)) as FamilySymbol;
                Level level = doc.GetElement(new ElementId(311)) as Level;
                Wall hostWall =doc.GetElement(new ElementId(354651)) as Wall;
                XYZ location = sel.PickPoint();
                FamilyInstance familyInstance = doc.Create.NewFamilyInstance(location, faiFamilySymbol,
                    hostWall, level, StructuralType.NonStructural);
                
                ts.Commit();
            }
            catch (Exception)
            {
                if (ts.GetStatus() == TransactionStatus.Started)
                {
                    ts.RollBack();
                }
            }

            return Result.Succeeded;
        }
    }
}