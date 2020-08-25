using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace CodeInTangsengjiewa3.BinLibrary.Extensions
{
    public static class ElementIdExtension
    {
        public static Element GetElement(this ElementId eleId, Document doc)
        {
            return doc.GetElement(eleId);
        }
    }
}