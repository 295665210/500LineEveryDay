using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using View = Autodesk.Revit.DB.View;

namespace DirectionCalculation
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class FindSouthFacingWallsWithoutProjectLocation : FindSouthFacingWalls, IExternalCommand
    {
        static AddInId m_appId = new AddInId(new Guid("8B29D56B-7B9A-4c79-8A38-B1C13B921877"));

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Application = commandData.Application.Application;
            Document = commandData.Application.ActiveUIDocument.Document;
            Transaction trans = new Transaction(Document, "FindSouthFacingWallsWithoutProjectLocation");
            trans.Start();
            Execute(false);
            CloseFile();
            trans.Commit();
            return Result.Succeeded;
        }
    }


    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class FindSouthFacingWallsWithProjectLocation : FindSouthFacingWalls, IExternalCommand
    {
        static AddInId m_appId = new AddInId(new Guid("6CADE602-7F32-496c-AA37-CEE4B0EE6087"));

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Application = commandData.Application.Application;
            Document = commandData.Application.ActiveUIDocument.Document;
            Transaction trans = new Transaction(Document, "FindSouthFacingWallsWithProjectLocation");
            trans.Start();
            Execute(true);

            CloseFile();
            trans.Commit();

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class FindSouthFacingWindowsWithoutProjectLocation : FindSouthFacingWindows, IExternalCommand
    {
        static AddInId m_appId = new AddInId(new Guid("AB3588F5-1CD1-4693-9DF0-C0890C811B21"));

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Application = commandData.Application.Application;
            Document = commandData.Application.ActiveUIDocument.Document;
            Transaction trans = new Transaction(Document, "FindSouthFacingWindowsWithoutProjectLocation");
            trans.Start();
            Execute(false);

            CloseFile();
            trans.Commit();
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class FindSouthFacingWindowsWithProjectLocation : FindSouthFacingWindows, IExternalCommand
    {
        static AddInId m_appId = new AddInId(new Guid("BFECDEA2-C384-4bcc-965E-EA302BA309AA"));

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Application = commandData.Application.Application;
            Document = commandData.Application.ActiveUIDocument.Document;
            Transaction trans = new Transaction(Document, "FindSouthFacingWindowsWithProjectLocation");
            trans.Start();

            Execute(true);

            CloseFile();
            trans.Commit();
            return Result.Succeeded;
        }
    }
}