using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CodeInTangsengjiewa4.CodeInFunny.FamilyLibrary
{
    class LoadFamilyExEvent : IExternalEventHandler
    {
        public string filePath = string.Empty;
        public bool result;


        public void Execute(UIApplication app)
        {
            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = uidoc.Document;
            Transaction ts = new Transaction(doc, "加载族");
            ts.Start();
            result = doc.LoadFamily(filePath);
            ts.Commit();
        }

        public string GetName()
        {
            return "加载族";
        }
    }
}