using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace CodeInTangsengjiewa4.CodeInFunny.FamilyLibrary
{
    class DBViewItem
    {
        public DBViewItem(View dbView, Document dbdoc)
        {
            ElementType viewType = dbdoc.GetElement(dbView.GetTypeId()) as ElementType;
            Name = viewType.Name + ":" + dbView.Name;
            Id = dbView.Id;
            UniqueId = dbView.UniqueId;
            View = dbView;
        }

        public override string ToString()
        {
            return Name;
        }

        public string Name { get; set; }
        public ElementId Id { get; set; }
        public string UniqueId { get; set; }
        public View View { get; set; }
    }
}