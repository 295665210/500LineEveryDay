using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Markup;
using Autodesk.Revit.DB;
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
#region Constructors
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
            DataList = new List<FieldData>();
            SchemaId = schemaId.ToString();
            ReadAccess = readAccess;
            WriteAccess = writeAccess;
            ApplicationId = applicationId;
            Name = name;
            Documentation = documentation;
        }
#endregion

#region Data addition
        /// <summary>
        /// Adds a new field to the wrapper's list of fields.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="typeIn"></param>
        /// <param name="unit"></param>
        /// <param name="subSchema"></param>
        public void AddData(string name, Type typeIn, UnitType unit, SchemaWrapper subSchema)
        {
            m_DataList.Add(new FieldData(name, typeIn.FullName, unit, subSchema));
        }
#endregion

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
        /// <summary>
        /// The write access of the Schema
        /// </summary>
        public AccessLevel ReadAccess
        {
            get { return m_ReadAccess; }
            set { m_ReadAccess = value; }
        }

        public AccessLevel WriteAccess
        {
            get { return m_WriteAccess; }
            set { m_WriteAccess = value; }
        }

        /// <summary>
        /// Vendor Id    [Vendor : 供应商，卖主]
        /// </summary>
        public string VendorId
        {
            get { return m_vendorId; }
            set { m_vendorId = value; }
        }
        /// <summary>
        /// Application id
        /// </summary>
        public string ApplicationId
        {
            get { return m_applicationId; }
            set { m_applicationId = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Documentation
        {
            get { return m_Documentation; }
            set { m_Documentation = value; }
        }
        /// <summary>
        /// The name of the Schema [Schema : 概要，图解，计划，模式]
        /// </summary>
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
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