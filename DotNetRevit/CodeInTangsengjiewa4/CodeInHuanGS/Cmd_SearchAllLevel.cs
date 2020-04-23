using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CodeInTangsengjiewa4.BinLibrary.Extensions;

namespace CodeInTangsengjiewa4.CodeInHuanGS
{
    [Transaction(TransactionMode.Manual)]
    public class Cmd_SearchAllLevel : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document document = commandData.Application.ActiveUIDocument.Document;
            GetLevelInfo(document);
            return Result.Succeeded;
        }

        public void GetLevelInfo(Document document)
        {
            StringBuilder levelInformation = new StringBuilder();
            int levelNumber = 0;
            FilteredElementCollector collector = new FilteredElementCollector(document);
            ICollection<Element> collection = collector.OfClass(typeof(Level)).ToElements();
            foreach (Element e in collection)
            {
                Level level = e as Level;
                if (null != level)
                {
                    levelNumber++;
                    levelInformation.Append("\nLevel Nanme:" + level.Name);

                    levelInformation.Append("\n\tElevation:" + string.Format("{0:000.000}",level.Elevation.FeetToMm()/1000));
                    // levelInformation.Append("\n\tElevation:" + string.Format("{0:000.00}",level.Elevation.FeetToMm()));
                }
            }

            levelInformation.Append("\n\nThere are " + levelNumber + " levels in the document");

            TaskDialog.Show("Info", levelInformation.ToString());
        }
    }
}