using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;


namespace ExtensibleStorageManager.User
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class UICommand : Window
    {
#region Constructor
        public UICommand(Document doc, string applicationId)
        {
            InitializeComponent();
            this.Closing += new System.ComponentModel.CancelEventHandler(UICommand_Closing);
            m_Document = doc;

            //Create a new empty schemaWrapper
            m_SchemaWrapper =
                SchemaWrapperTools.SchemaWrapper.NewSchema(Guid.Empty, AccessLevel.Public, AccessLevel.Public,
                    "adsk", applicationId, "schemaName", "Schema documentation");
            this.m_label_applicationAppId.Content = applicationId;
            UpdateUI();
        }
#endregion


#region  UI Handers
        /// <summary>
        /// Store the guid of the last-used schema in the application object for convenient access
        /// later if the user re-creates and displays this dialog again.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void UICommand_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.LastGuid = m_textBox_SchemaId.Text;
        }


        /// <summary>
        /// put  a new ,arbitrary Guid in the schema text box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_button_NewSchemaId_Click(object sender, RoutedEventArgs e)
        {
            m_textBox_SchemaId.Text = StorageCommand.NewGuid().ToString();
        }

        /// <summary>
        /// Handler for the "Create a simple schema" button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_button_CreateSetSaveSimple_Click(object sender, RoutedEventArgs e)
        {
            CreateSetSave(SampleSchemaComplexity.SimpleExample);
        }

        private void m_button_CreateSetSaveComplex_Click(object sender, RoutedEventArgs e)
        {
            CreateSetSave(SampleSchemaComplexity.ComplexExample);
        }

        private void CreateSetSave(SampleSchemaComplexity schemaComplexity)
        {
            //Get read-write access levels and schema and application Ids from the active dialog
            AccessLevel read;
            AccessLevel write;
            GetUIAccessLevels(out read, out write);
            if (!ValidateGuids())
            {
                TaskDialog.Show("ExtensibleStorage Manager", "Invalid Schema or ApplicationId Guid.");
                return;
            }

            //Get a pathname for an XML file from the user.
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.DefaultExt = ".xml";
            sfd.Filter = "SchemaWrapper Xml files (*.xml)|*.xml";
            sfd.InitialDirectory = GetStartingXmlPath();

            sfd.FileName = this.m_textBox_SchemaName.Text + "_" + this.m_textBox_SchemaVendorId.Text + "___" +
                this.m_textBox_SchemaId.Text.Substring(31) + ".xml";

            Nullable<bool> result = sfd.ShowDialog();

            if ((result.HasValue) && (result == true))
            {
                try
                {
                    //Create a new sample SchemaWrapper, schema, and Entity and store it in the current document's ProjectInformation element.
                    m_SchemaWrapper = StorageCommand.CreateSetAndExport(m_Document.ProjectInformation, sfd.FileName,
                        new Guid(this.m_textBox_SchemaId.Text), read, write, this.m_textBox_SchemaVendorId.Text,
                        this.m_textBox_SchemaApplicationId.Text, this.m_textBox_SchemaName.Text,
                        this.m_textBox_SchemaDocumentation.Text, schemaComplexity);
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("ExtensibleStorage Manager", "Could not Create Schema.  " + ex.ToString());
                    return;
                }

                UpdateUI();

                //Display the schema fields and sample data we just created in a dialog.
                ExtensibleStorageManager.User.UIData dataDialog = new UIData();
                string schemaData = this.m_SchemaWrapper.ToString();
                string entityData =
                    this.m_SchemaWrapper.GetSchemaEntityData(
                        m_Document.ProjectInformation.GetEntity(m_SchemaWrapper.GetSchema()));
                string allData = "Schema: " + Environment.NewLine + schemaData + Environment.NewLine +
                    Environment.NewLine + "Entity" + Environment.NewLine + entityData;
                dataDialog.SetData(allData);
                dataDialog.ShowDialog();
            }
        }

        private void m_button_EditExistingSimple_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StorageCommand.EditExistingData(m_Document.ProjectInformation, new Guid(m_textBox_SchemaId.Text),
                    out m_SchemaWrapper);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("ExtensibleStorage Manager", "Could not extract data from schema. " + ex.ToString());
                return;
            }
            UpdateUI();
            ///Display the schema fields and new data in a separate dialog.
            UIData dataDialog = new UIData();
            string schemaData = this.m_SchemaWrapper.ToString();
            string entityData =
                this.m_SchemaWrapper.GetSchemaEntityData(
                    m_Document.ProjectInformation.GetEntity(m_SchemaWrapper.GetSchema()));
            string allData = "Schema: " + Environment.NewLine + schemaData + Environment.NewLine + Environment.NewLine +
                "Entity" + Environment.NewLine + entityData;

            dataDialog.SetData(allData);
            dataDialog.ShowDialog();
        }

        private void m_button_LookupExtract_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Given a Guid that corresponds to a schema that already exists in a document, create a SchemaWrapper
                //from it and display its top-level data in the dialog.
                StorageCommand.LookupAndExtractData(m_Document.ProjectInformation, new Guid(m_textBox_SchemaId.Text),
                    out m_SchemaWrapper);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("ExtensibleStorage Manager", "Could not extract data from Schema.  " + ex.ToString());
                return;
            }
            UpdateUI();
            UIData dataDialog = new UIData();

            //Get and display the schema field data and the actual entity data in a separate dialog.
            string schemaData = this.m_SchemaWrapper.ToString();
            string entityData =
                this.m_SchemaWrapper.GetSchemaEntityData(
                    m_Document.ProjectInformation.GetEntity(m_SchemaWrapper.GetSchema()));
            string allData = "Schema: " + Environment.NewLine + schemaData + Environment.NewLine + Environment.NewLine +
                "Entity" + Environment.NewLine + entityData;

            dataDialog.SetData(allData);
            dataDialog.ShowDialog();
        }

        private void m_button_CreateWrapperFromSchema_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Given a Guid that corresponds to a schema that already exists in a document, create a SchemaWrapper
                //from it and display its top-level data in the dialog.
                StorageCommand.CreateWrapperFromSchema(new Guid(m_textBox_SchemaId.Text), out m_SchemaWrapper);
                UpdateUI();
            }

            catch (Exception ex)
            {
                TaskDialog.Show("ExtensibleStorage Manager",
                    "Could not Create SchemaWrapper from Schema.  " + ex.ToString());
                return;
            }
            //Display all of the schema's field data in a separate dialog.
            UIData dataDialog = new UIData();
            dataDialog.SetData(m_SchemaWrapper.ToString());
            dataDialog.ShowDialog();
        }
#endregion

#region  Helper methods
        /// <summary>
        /// return a convenient recommended path to save schema files in.
        /// </summary>
        /// <returns></returns>
        private string GetStartingXmlPath()
        {
            string currentAssembly = Assembly.GetAssembly(this.GetType()).Location;
            return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(currentAssembly), "schemas");
        }

        /// <summary>
        /// Synchronize all UI controls in the dialog with the data in m_SchemaWrapper.
        /// </summary>
        private void UpdateUI()
        {
            this.m_textBox_SchemaApplicationId.Text = m_SchemaWrapper.Data.ApplicationId;
            this.m_textBox_SchemaVendorId.Text = m_SchemaWrapper.Data.VendorId;
            this.m_textBox_SchemaPath.Content = m_SchemaWrapper.GetXmlPath();
            this.m_textBox_SchemaName.Text = m_SchemaWrapper.Data.Name;
            this.m_textBox_SchemaDocumentation.Text = m_SchemaWrapper.Data.Documentation;
            this.m_textBox_SchemaId.Text = m_SchemaWrapper.Data.SchemaId;
            if (this.m_textBox_SchemaId.Text == Guid.Empty.ToString())
                this.m_textBox_SchemaId.Text = Application.LastGuid;

            switch (m_SchemaWrapper.Data.ReadAccess)
            {
            case AccessLevel.Application:
            {
                m_rb_ReadAccess_Application.IsChecked = true;
                break;
            }
            case AccessLevel.Public:
            {
                m_rb_ReadAccess_Public.IsChecked = true;
                break;
            }
            case AccessLevel.Vendor:
            {
                m_rb_ReadAccess_Vendor.IsChecked = true;
                break;
            }
            }

            switch (m_SchemaWrapper.Data.WriteAccess)
            {
            case AccessLevel.Application:
            {
                m_rb_WriteAccess_Application.IsChecked = true;
                break;
            }
            case AccessLevel.Public:
            {
                m_rb_WriteAccess_Public.IsChecked = true;
                break;
            }
            case AccessLevel.Vendor:
            {
                m_rb_WriteAccess_Vendor.IsChecked = true;
                break;
            }
            }
        }

        /// <summary>
        /// Retrieve AccessLevel enums for read and write permissions from the UI
        /// </summary>
        /// <param name="read"></param>
        /// <param name="write"></param>
        private void GetUIAccessLevels(out AccessLevel read, out AccessLevel write)
        {
            read = AccessLevel.Public;
            write = AccessLevel.Public;

            if (m_rb_ReadAccess_Application.IsChecked == true)
            {
                read = AccessLevel.Application;
            }

            else if (m_rb_ReadAccess_Public.IsChecked == true)
            {
                read = AccessLevel.Public;
            }
            else
            {
                read = AccessLevel.Vendor;
            }
            if (m_rb_WriteAccess_Application.IsChecked == true)
                write = AccessLevel.Application;
            else if (m_rb_WriteAccess_Public.IsChecked == true)
                write = AccessLevel.Public;
            else
                write = AccessLevel.Vendor;
        }

        private bool ValidateGuids()
        {
            bool retval = true;
            try
            {
                Guid schemaId = new Guid(this.m_textBox_SchemaId.Text);
                Guid applicationId = new Guid(this.m_textBox_SchemaApplicationId.Text);
            }
            catch (Exception)
            {
                retval = false;
            }
            return retval;
        }
#endregion

#region Properties
        /// <summary>
        /// The active document in Revit that the dialog queries for Schema and Entity data.
        /// </summary>
        public Document Document
        {
            get { return m_Document; }
            set { m_Document = value; }
        }
#endregion

#region  Data
        private SchemaWrapperTools.SchemaWrapper m_SchemaWrapper;

        private Document m_Document;
#endregion
    }
}