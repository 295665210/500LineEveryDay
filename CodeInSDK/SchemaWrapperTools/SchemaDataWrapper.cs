using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.DB.ExtensibleStorage;

namespace SchemaWrapperTools
{
    /// <summary>
    /// A class to store a list of FieldData objects as well as the top level data(name,access levels ,SchemaId, etc...)
    /// of an Autodesk.Revit.DB.ExtensibleStorage.Schema
    /// </summary>
    [Serializable]
    public class SchemaDataWrapper
    {
        /// <summary>
        /// For serialization only - Do not use.
        /// </summary>
        internal SchemaDataWrapper() { }

        public SchemaDataWrapper
        (
            Guid schemaId, AccessLevel readAccess, AccessLevel writeAccess, string vendorId, string applicationId,
            string name, string documentation
        )
        {
            
        }


#region Properties
        /// <summary>
        /// The list of FieldData objects in the wrapper
        /// </summary>
        public List<FieldData> DataList
        {
            get { return m_DataList; }
            set { m_DataList = value; }
        }
        /// <summary>
        /// The schemaId Guid of the Schema
        /// </summary>
        public string SchemaId
        {
            get { return m_schemaId; }
            set { m_schemaId = value; }
        }





#endregion


#region Data
        private AccessLevel m_ReadAccess;
        private AccessLevel m_WriteAccess;
        private List<FieldData> m_DataList;
        private string m_applicationId;
        private string m_schemaId;
        private string m_vendorId;
        private string m_Name;
        private string m_Documentation;
#endregion
    }
}