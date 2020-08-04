using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using View = Autodesk.Revit.DB.View;

namespace SchemaWrapperTools
{
    /// <summary>
    /// A class to store schema field information
    /// </summary>
    [Serializable]
    public class FieldData
    {
#region  Constructors
        /// <summary>
        /// For serialization only -- Do not use.
        /// </summary>
        internal FieldData()
        {
        }

        /// <summary>
        /// Create a new FieldData object
        /// </summary>
        /// <param name="name"></param>
        /// <param name="typeIn"></param>
        /// <param name="unit"></param>
        public FieldData(string name, string typeIn, UnitType unit) : this(name, typeIn, unit, null)
        {
        }


        public FieldData(string name, string typeIn, UnitType unit, SchemaWrapper subSchema)
        {
            m_Name = name;
            m_Type = typeIn;
            m_Unit = unit;
            m_SubSchema = subSchema;
        }
#endregion

#region Other helper functions
        public override string ToString()
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("   Field: ");
            strBuilder.Append(Name);
            strBuilder.Append(", ");
            strBuilder.Append(Type);
            strBuilder.Append(", ");
            strBuilder.Append(Unit.ToString());

            if (SubSchema != null)
            {
                strBuilder.Append(Environment.NewLine + " " + SubSchema.ToString());
            }
            return strBuilder.ToString();
        }
#endregion

#region Properties
        /// <summary>
        /// The string representation of a schema field type(e.g. System.Int32)
        /// </summary>
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        /// <summary>
        /// The string representation of schema field type (e.g. System.Int32)
        /// </summary>
        public string Type
        {
            get { return m_Type; }
            set { m_Type = value; }
        }

        /// <summary>
        /// The unit type of a field
        /// </summary>
        public UnitType Unit
        {
            get { return m_Unit; }
            set { m_Unit = value; }
        }

        public SchemaWrapper SubSchema
        {
            get { return m_SubSchema; }
            set { m_SubSchema = value; }
        }
#endregion

#region Data
        private SchemaWrapper m_SubSchema;
        private string m_Name;
        private string m_Type;
        private UnitType m_Unit;
#endregion
    }
}