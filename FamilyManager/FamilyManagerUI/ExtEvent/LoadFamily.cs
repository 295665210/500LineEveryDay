using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using QiShiLog;
using QiShiLog.Log;

namespace FamilyManagerUI.ExtEvent
{
    class LoadFamily : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            throw new NotImplementedException();
        }

        public string GetName()
        {
            return "LoadFamily";
        }
    }
}
