using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CodeInTangsengjiewa4.General
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_ForcedDisplay : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            View acView = doc.ActiveView;
            Transaction ts = new Transaction(doc, "***");
            ts.Start();
            try
            {
                FilteredElementCollector collector = new FilteredElementCollector(doc);
                var list = new List<ElementId>();
                list = collector.WhereElementIsNotElementType().Select(m => m.Id).ToList();
                MessageBox.Show(list.Count.ToString());
                acView.UnhideElements(list);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                if (ts.GetStatus() == TransactionStatus.Started)
                {
                    ts.RollBack();
                }
            }
            ts.Commit();
            return Result.Succeeded;
        }
    }
}