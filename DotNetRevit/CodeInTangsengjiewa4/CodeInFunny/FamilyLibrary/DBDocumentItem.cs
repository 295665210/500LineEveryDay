using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace CodeInTangsengjiewa4.CodeInFunny.FamilyLibrary
{
    class DBDocumentItem
    {
        public bool IsNull { get; set; }
        public string Name { get; set; }
        public Document Document { get; set; }

        public DBDocumentItem(string name, Document db)
        {
            this.Name = name;
            this.Document = db;
            IsNull = false;
        }

        public DBDocumentItem()
        {
            IsNull = true;
        }

        public override string ToString()
        {
            if (IsNull)
            {
                return "open document";
            }
            return Name;
        }
    }
}
