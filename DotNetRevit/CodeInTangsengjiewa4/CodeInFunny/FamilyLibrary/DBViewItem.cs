using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Markup;
using Autodesk.Revit.DB;
using View = Autodesk.Revit.DB.View;

namespace CodeInTangsengjiewa4.CodeInFunny.FamilyLibrary
{
    class DBViewItem
    {
        public string Name { get; set; }
        public ElementId Id { get; set; }
        public string UniqueId { get; set; }
        public View View { get; set; }

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
    }
}