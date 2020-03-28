using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;

namespace CodeInTangsengjiewa4.CodeInFunny.FamilyLibrary
{
    class PreviewExEvent:IExternalEventHandler
    {
        public string familyPath = string.Empty;



        public void Execute(UIApplication app)
        {
            throw new NotImplementedException();
        }

        public string GetName()
        {
            return "预览族模型";
        }
    }
}
