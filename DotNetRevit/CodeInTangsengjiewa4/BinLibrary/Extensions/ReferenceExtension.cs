using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace CodeInTangsengjiewa4.BinLibrary.Extensions
{
    public static class ReferenceExtension
    {
        /// <summary>
        /// 从引用获得element
        /// </summary>
        /// <param name="thisRef"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static Element GetElement(this Reference thisRef, Document doc)
        {
            return doc.GetElement(thisRef);
        }
    }
}