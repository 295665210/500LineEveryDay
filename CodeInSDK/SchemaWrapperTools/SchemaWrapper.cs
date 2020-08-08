using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;

namespace SchemaWrapperTools
{
    public class SchemaWrapper
    {
#region Constructors and class Factories
        /// <summary>
        /// For serialization only - Do not use.[serialization : 序列化？ 初始化？]
        /// </summary>
        internal SchemaWrapper() { }

        /// <summary>
        ///  Creates a new SchemaWrapper from an existing schema.  
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        public static SchemaWrapper FromSchema(Schema schema)
        {
            SchemaWrapper swReturn = new SchemaWrapper(schema);
            foreach (Field currentField in schema.ListFields())
            {
                //we need to add call AddField on the SchemaWrapper we are creating for each field in the source schema.
                //since we do not know the data type of the field yet, we need  to get the generic method first and then query the field data types from the field data types and instantiate a new generic method with those types an parameters.

                //Get the "AddField" method.
                MethodInfo addFieldmethod = typeof(SchemaWrapper).GetMethod("AddField",
                    new Type[] {typeof(string), typeof(UnitType), typeof(SchemaWrapper)});
                Type[] methodGenericParameters = null;

                //Get he generic type parameters. The type will either be a single type,an IList<> of a single type, or an IDictionary<> of a key type and a value type.
                if (currentField.ContainerType == ContainerType.Simple)
                {
                    methodGenericParameters = new Type[] {currentField.ValueType};
                }
                else if (currentField.ContainerType == ContainerType.Array)
                {
                    methodGenericParameters = new Type[]
                    {
                        typeof(IList<int>).GetGenericTypeDefinition()
                            .MakeGenericType(new Type[] {currentField.ValueType})
                    };
                }
                else
                {
                    methodGenericParameters = new Type[]
                    {
                        typeof(IList<int>).GetGenericTypeDefinition().MakeGenericType(new Type[]
                            {currentField.KeyType, currentField.ValueType})
                    };
                }

                //Instantiate a new generic method from the "AddField" method we got before with the generic parameters we got from the current field we are querying.
                MethodInfo genericAddFieldMethodInstantiated =
                    addFieldmethod.MakeGenericMethod(methodGenericParameters);
                SchemaWrapper
                    swSub = null; //Our subSchema is null by default unless the field is of type "Entity", in which case we will call "FromSchema" again on the field's subSchema.
                if (currentField.ValueType == typeof(Entity))
                {
                    Schema subSchema = Schema.Lookup(currentField.SubSchemaGUID);
                    swSub = SchemaWrapper.FromSchema(subSchema);
                }

                //Invoke the "AddField" method with the generic parameters from the current field.
                genericAddFieldMethodInstantiated.Invoke(swReturn,
                    new object[] {currentField.FieldName, currentField.UnitType, swSub});
            }
            return swReturn;
        }

        /// <summary>
        /// Creates a new SchemaWrapper from a Guid.
        /// </summary>
        /// <param name="schemaId"></param>
        /// <param name="readAccess"></param>
        /// <param name="writeAccess"></param>
        /// <param name="vendorId"></param>
        /// <param name="applicationId"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static SchemaWrapper NewSchema
        (
            Guid schemaId, AccessLevel readAccess, AccessLevel writeAccess, string vendorId, string applicationId,
            string name, string description
        )
        {
            return new SchemaWrapper(schemaId, readAccess, writeAccess, vendorId, applicationId, name, description);
        }


        /// <summary>
        /// Creates a new SchemaWrapper from a XML file on disk.
        /// </summary>
        /// <param name="xmlDataPath"></param>
        /// <returns></returns>
        public static SchemaWrapper FromXml(string xmlDataPath)
        {
            XmlSerializer sampleSchemaInXml = new XmlSerializer(typeof(SchemaWrapper));
            Stream streamXmlIn = new FileStream(xmlDataPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            SchemaWrapper wrapperIn = null;
            try
            {
                wrapperIn = sampleSchemaInXml.Deserialize(streamXmlIn) as SchemaWrapper;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Could not deserialize schema file." + ex.ToString());
                return null;
            }
            wrapperIn.SetRevitAssembly();
            streamXmlIn.Close();
            try
            {
                wrapperIn.FinishSchema();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Could not create schema." + ex.ToString());
                return null;
            }
            return wrapperIn;
        }


        /// <summary>
        /// Constructor used by class factories.
        /// </summary>
        /// <param name="schemaId"></param>
        /// <param name="readAccess"></param>
        /// <param name="writeAccess"></param>
        /// <param name="vendorId"></param>
        /// <param name="applicationId"></param>
        /// <param name="schemaName"></param>
        /// <param name="schemaDescription"></param>
        private SchemaWrapper
        (
            Guid schemaId, AccessLevel readAccess, AccessLevel writeAccess, string vendorId, string applicationId,
            string schemaName, string schemaDescription
        )
        {
            m_SchemaDataWrapper = new SchemaDataWrapper(schemaId, readAccess, writeAccess, vendorId, applicationId,
                schemaName, schemaDescription);
            SetRevitAssembly();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schema"></param>
        private SchemaWrapper
            (Schema schema) : this(schema.GUID, schema.ReadAccessLevel, schema.WriteAccessLevel, schema.VendorId,
            schema.ApplicationGUID.ToString(), schema.SchemaName, schema.Documentation)
        {
            this.SetSchema(schema);
        }
#endregion

#region Active schema manipulation
        //[manipulation : 操作，处理]
        public void AddField<TypeName>(string name, UnitType unit, SchemaWrapper subSchema)
        {
            m_SchemaDataWrapper.AddData(name, typeof(TypeName), unit, subSchema);
        }

        public void FinishSchema()
        {
            //Create the Autodesk.Revit.DB.ExtensibleStorage.SchemaBuilder that actually builds the schema
            m_SchemaBuilder = new SchemaBuilder(new Guid(m_SchemaDataWrapper.SchemaId));
            //we will add a new field to our schema from each FieldData object in our SchemaWrapper.
            foreach (FieldData currentFieldData in m_SchemaDataWrapper.DataList)
            {
                //if the current field's type is a supported system type (int, string ,etc...)
                //we can instantiate it with Type.GetType(). If the current field's type is a supported Revit API type
                //(XYZ,elementID,etc...),we need to call GetType from the RevitAPI.dll assembly object.
                Type fieldType = null;
                try
                {
                    fieldType = Type.GetType(currentFieldData.Type, true, true);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    try
                    {
                        //get the field from the revit API assembly.
                        fieldType = m_Assembly.GetType(currentFieldData.Type);
                    }
                    catch (Exception exx)
                    {
                        Debug.WriteLine("Error getting type: " + exx.ToString());
                        throw;
                    }
                }

                // Now that we have the data type of field we need to add, we need to call either SchemaBuilder.AddSimpleFiled, AddArrayField, or AddMapField.
                FieldBuilder currentFieldBuilder = null;
                Guid subSchemaId = Guid.Empty;
                Type[] genericParams = null;

                if (currentFieldData.SubSchema != null)
                {
                    subSchemaId = new Guid(currentFieldData.SubSchema.Data.SchemaId);
                }

                // If out data type is a  generic ,it is an IList<> or an IDictionary<>, so it's an array or map type
                if (fieldType.IsGenericType)
                {
                    Type tGeneric =
                        fieldType.GetGenericTypeDefinition(); //tGeneric will be either an IList<> or an IDictionary<>.
                    //Create an IList<> or an IDictionary<> to compare against tGeneric.
                    Type iDictionaryType = typeof(IDictionary<int, int>).GetGenericTypeDefinition();
                    Type iListType = typeof(IList<int>).GetGenericTypeDefinition();

                    genericParams =
                        fieldType
                            .GetGenericArguments(); // Get the actual data type(s) stored in the field's IList<> or IDictionary<>
                    if (tGeneric == iDictionaryType)
                    {
                        //Pass the key and value type of our dictionary type to AddMapField
                        currentFieldBuilder =
                            m_SchemaBuilder.AddMapField(currentFieldData.Name, genericParams[0], genericParams[1]);
                    }
                    else if (tGeneric == iListType)
                    {
                        //Pass the value type of our list type to AddArrayField
                        currentFieldBuilder = m_SchemaBuilder.AddArrayField(currentFieldData.Name, genericParams[0]);
                    }
                    else
                    {
                        throw new Exception("Generic type is neither IList<> nor IDictionary<>, cannot process.");
                    }
                }
                else
                {
                    //The simpler case -- just add field using a name and a System.Type.
                    currentFieldBuilder = m_SchemaBuilder.AddSimpleField(currentFieldData.Name, fieldType);
                }

                if ((fieldType == (typeof(Entity)))
                    || (fieldType.IsGenericType && ((genericParams[0] == (typeof(Entity)))
                    || ((genericParams.Length > 1) && (genericParams[1] == typeof(Entity))))))
                {
                    currentFieldBuilder.SetSubSchemaGUID(subSchemaId); //set the SubSchemaId if our field
                    currentFieldData.SubSchema
                        .FinishSchema(); //Recursively create the schema for the subSchema.
                }
                if (currentFieldData.Unit != UnitType.UT_Undefined)
                {
                    currentFieldBuilder.SetUnitType(currentFieldData.Unit);
                }
            }

            //Set all the top level data in the schema we are generating
            m_SchemaBuilder.SetReadAccessLevel(this.Data.ReadAccess);
            m_SchemaBuilder.SetWriteAccessLevel(this.Data.WriteAccess);
            m_SchemaBuilder.SetVendorId(this.Data.VendorId);
            m_SchemaBuilder.SetApplicationGUID(new Guid(this.Data.ApplicationId));
            m_SchemaBuilder.SetDocumentation(this.Data.Documentation);
            m_SchemaBuilder.SetSchemaName(this.Data.Name);

            //Actually finish creating the Autodesk.Revit.DB.ExtensibleStorage.Schema
            m_Schema = m_SchemaBuilder.Finish();
        }

        /// <summary>
        /// Serializes a SchemaWrapper to an Xml file.
        /// </summary>
        /// <param name="xmlDataPath"></param>
        public void ToXml(string xmlDataPath)
        {
            XmlSerializer sampleSchemaOutXml = new XmlSerializer(typeof(SchemaWrapper));
            Stream streamXmlOut = new FileStream(xmlDataPath, FileMode.OpenOrCreate, FileAccess.ReadWrite,
                FileShare.ReadWrite);
            sampleSchemaOutXml.Serialize(streamXmlOut, this);
            streamXmlOut.Close();
            return;
        }

        public override string ToString()
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.AppendLine("--Start Schema-- " + " Name: " + this.Data.Name + ", Description: " +
                this.Data.Documentation + ", Id: " + this.Data.SchemaId + ", ReadAccess: " +
                this.Data.ReadAccess.ToString() + ", WriteAccess: " + this.Data.WriteAccess.ToString());
            foreach (FieldData fd in this.Data.DataList)
            {
                strBuilder.AppendLine(fd.ToString());
            }
            strBuilder.AppendLine("--End Schema--");
            return strBuilder.ToString();
        }
#endregion


#region Helper Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public string GetSchemaEntityData(Entity entity)
        {
            StringBuilder swBuilder = new StringBuilder();
            DumpAllSchemaEntityData<Entity>(entity, entity.Schema, swBuilder);
            return swBuilder.ToString();
        }


        private void DumpAllSchemaEntityData<EntityType>
            (EntityType storageEntity, Schema schema, StringBuilder strBuilder)
        {
            strBuilder.AppendLine("Schema/Entity Name: " + "" + ", Description: " + schema.Documentation + ", Id:" +
                schema.GUID.ToString() + ", Read Access: " + schema.ReadAccessLevel.ToString() + ", Write Access : " +
                schema.WriteAccessLevel.ToString());
            foreach (Field currentField in schema.ListFields())
            {
                //Here, we call GetFieldDataString on this class, the SchemaWrapper class. However, we must call it with specific generic parameters that are only known at runtime, so we must use reflection to dynamically create a method with parameters from the current field we want to extract data from.
                ParameterModifier[]
                    pmodifiers =
                        new ParameterModifier[0]; //a Dummy parameter needed for GetMethod() call,empty. [Dummy : 空的，无意义的。]
                //Get the method.
                MethodInfo getFieldDataAsStringMethod = typeof(SchemaWrapper).GetMethod("GetFieldDataAsString",
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod, Type.DefaultBinder,
                    new Type[] {typeof(Field), typeof(Entity), typeof(StringBuilder)}, pmodifiers);

                Type[] methodGenericParameters = null;
                if (currentField.ContainerType == ContainerType.Simple)
                {
                    methodGenericParameters =
                        new Type[] {typeof(int), currentField.ValueType}; //dummy int types for non dictionary type.
                }

                else if (currentField.ContainerType == ContainerType.Array)
                {
                    methodGenericParameters =
                        new Type[] {typeof(int), currentField.ValueType}; // dummy int types for non dictionary type.
                }
                else
                {
                    methodGenericParameters = new Type[] {currentField.KeyType, currentField.ValueType};
                }

                //instantiate a generic version of "GetFieldDataAsString" whit type parameters we just got from our field.
                MethodInfo genericGetFieldDataAsStringmethodInstantiated =
                    getFieldDataAsStringMethod.MakeGenericMethod(methodGenericParameters);

                //Call that method to get the data out of that field.
                genericGetFieldDataAsStringmethodInstantiated.Invoke(this,
                    new object[] {currentField, storageEntity, strBuilder});
            }
            strBuilder.AppendLine("------------------");
        }

        /// <summary>
        /// Recursively gets all data from a field and appends it in string format to a StrBuilder.[Recursively : 递归的]
        /// </summary>
        /// <typeparam name="KeyType"></typeparam>
        /// <typeparam name="FieldType"></typeparam>
        /// <param name="field"></param>
        /// <param name="entity"></param>
        /// <param name="strBuilder"></param>
        private void GetFieldDataAsString<KeyType, FieldType>(Field field, Entity entity, StringBuilder strBuilder)
        {
            string fieldName = field.FieldName;
            System.Type fieldType = field.ValueType;
            UnitType fieldUnit = field.UnitType;
            ContainerType fieldContainerType = field.ContainerType;
            Type[] methodGenericParameters = null;
            object[] invokeParams = null;
            Type[] methodOverloadSelectionParams = null;
            if (field.ContainerType == ContainerType.Simple)
                methodGenericParameters = new Type[] {field.ValueType};
            else if (field.ContainerType == ContainerType.Array)
                methodGenericParameters = new Type[]
                    {typeof(IList<int>).GetGenericTypeDefinition().MakeGenericType(new Type[] {field.ValueType})};
            else //map
                methodGenericParameters = new Type[]
                {
                    typeof(IDictionary<int, int>).GetGenericTypeDefinition()
                        .MakeGenericType(new Type[] {field.KeyType, field.ValueType})
                };

            if (fieldUnit == UnitType.UT_Undefined)
            {
                methodOverloadSelectionParams = new Type[] {typeof(Field)};
                invokeParams = new object[] {field};
            }

            else
            {
                methodOverloadSelectionParams = new Type[] {typeof(Field), typeof(DisplayUnitType)};
                invokeParams = new object[] {field, DisplayUnitType.DUT_METERS};
            }

            MethodInfo instantiatedGenericGetMethod = entity.GetType().GetMethod("Get", methodOverloadSelectionParams)
                .MakeGenericMethod(methodGenericParameters);

            if (field.ContainerType == ContainerType.Simple)
            {
                FieldType retval = (FieldType) instantiatedGenericGetMethod.Invoke(entity, invokeParams);
                if (fieldType == typeof(Entity))
                {
                    Schema subSchema = Schema.Lookup(field.SubSchemaGUID);
                    strBuilder.AppendLine("Field: " + field.FieldName + ", Type: " + field.ValueType.ToString() +
                        ", value: " + " {SubEntity} " + ", Unit: " + field.UnitType.ToString() + ", ContainerType: " +
                        field.ContainerType.ToString());
                    DumpAllSchemaEntityData<FieldType>(retval, subSchema, strBuilder);
                }
                else
                {
                    string sRetval = retval.ToString();
                    strBuilder.AppendLine("Field: " + field.FieldName + ", Type" + field.ValueType.ToString() +
                        ", ContainerType : " + field.ContainerType.ToString());
                }
            }

            else if (field.ContainerType == ContainerType.Array)
            {
                IList<FieldType> listRetval =
                    (IList<FieldType>) instantiatedGenericGetMethod.Invoke(entity, invokeParams);
                if (fieldType == typeof(Entity))
                {
                    strBuilder.AppendLine("Field: " + field.FieldName + ", Type: " + field.ValueType.ToString() +
                        ", Value: " + "{SubEntity}" + ", Unit: " + field.UnitType.ToString() + ", ContainerType: " +
                        field.ContainerType.ToString());

                    foreach (FieldType fa in listRetval)
                    {
                        strBuilder.Append(" Array Value: ");
                        DumpAllSchemaEntityData<FieldType>(fa, Schema.Lookup(field.SubSchemaGUID), strBuilder);
                    }
                }
                else
                {
                    strBuilder.AppendLine("Field: " + field.FieldName + ", Type: " + field.ValueType.ToString() +
                        ", Value: " + " {Array} " + ", Unit: " + field.UnitType.ToString() + ", ContainerType: " +
                        field.ContainerType.ToString());
                    foreach (FieldType fa in listRetval)
                    {
                        strBuilder.AppendLine(" Array valueL " + fa.ToString());
                    }
                }
            }

            else //Map
            {
                strBuilder.AppendLine("Field: " + field.FieldName + ", Type: " + field.ValueType.ToString() +
                    ", Value: " + "{Map}" + ", Unit: " + field.UnitType.ToString() + ", ContainerType: " +
                    field.ContainerType.ToString());
                IDictionary<KeyType, FieldType> mapRetval =
                    (IDictionary<KeyType, FieldType>) instantiatedGenericGetMethod.Invoke(entity, invokeParams);
                if (fieldType == (typeof(Entity)))
                {
                    strBuilder.AppendLine("Field: " + field.FieldName + ", Type: " + field.ValueType.ToString() +
                        ", Value: " + "{SubEntity}" + ", Unit: " + field.UnitType.ToString() + ", ContainerType :" +
                        field.ContainerType.ToString());

                    foreach (FieldType fa in mapRetval.Values)
                    {
                        strBuilder.Append(" Map Value:");
                        DumpAllSchemaEntityData<FieldType>(fa, Schema.Lookup(field.SubSchemaGUID), strBuilder);
                    }
                }
                else
                {
                    strBuilder.AppendLine("Field: " + field.FieldName + ", Type: " + field.ValueType.ToString() +
                        ", Value: " + " {Map} " + ", Unit: " + field.UnitType.ToString() + ", ContainerType: " +
                        field.ContainerType.ToString());
                    foreach (FieldType fa in mapRetval.Values)
                    {
                        strBuilder.AppendLine(" Map value: " + fa.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// creates an instance of the RevitAPI.dll assembly so we can query type information from it.
        /// </summary>
        private void SetRevitAssembly()
        {
            m_Assembly = Assembly.GetAssembly(typeof(XYZ));
        }
#endregion

#region  properties
        /// <summary>
        /// Gets the Autodesk.DB.ExtensibleStorage Schema that the wrapper owns.
        /// </summary>
        /// <returns></returns>
        public Schema GetSchema()
        {
            return m_Schema;
        }

        /// <summary>
        /// Sets the Autodesk.DB.ExtensibleStorage schema that the wrapper owns.
        /// </summary>
        /// <param name="schema"></param>
        public void SetSchema(Schema schema)
        {
            m_Schema = schema;
        }

        /// <summary>
        /// Gets and set the SchemaDataWrapper of the SchemaWrapper. "Set" is for serialization use only.
        /// </summary>
        public SchemaDataWrapper Data
        {
            get { return m_SchemaDataWrapper; }
            set { m_SchemaDataWrapper = value; }
        }

        /// <summary>
        /// Get the path of the xml file this Schema was generated from.
        /// </summary>
        /// <returns></returns>
        public string GetXmlPath()
        {
            return m_xmlPath;
        }

        /// <summary>
        /// Set the path of the xml file this Schema wa generated from.
        /// </summary>
        /// <param name="path"></param>
        public void SetXmlPath(string path)
        {
            m_xmlPath = path;
        }
#endregion

#region Data
        private SchemaDataWrapper m_SchemaDataWrapper;


        /*
        1 ///https://www.cnblogs.com/ryanzheng/p/11075105.html 序列化相关的介绍。 
        2 序列化的用处介绍： 
        2.1 序列化是用来通信的，服务端把数据序列化，发送到客户端，客户端把接收到的数据反序列化后对数据进行操作，完成后再序列化发送到服务端，服务端再反序列化数据后对数据进行操作。 
        2.2 说白了，数据序列化以后在服务端和客户端之间传输。这个服务端和客户端的概念是广义的，可以在网络上，也可以在同一台机器的不同进程中，甚至在同一个进程中进行通信。在传统编程中，对象是通过调用栈间接的与客户端交互，但在面向服务的编程中，客户端永远都不会直接调用实例。
        2.3  不序列化也可以传输，但是无法跨平台.
        https://blog.csdn.net/shongyu/article/details/16337401
        */

        [NonSerialized] private Schema m_Schema;

        [NonSerialized] private SchemaBuilder m_SchemaBuilder;

        [NonSerialized] private Assembly m_Assembly;

        [NonSerialized] private string m_xmlPath;
#endregion
    }
}