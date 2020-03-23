using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;

namespace CodeInTangsengjiewa4.Test
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_ExternalStorageDataTest : IExternalCommand
    {
        private UIApplication uiapp = null;
        private UIDocument uidoc = null;
        private Document doc = null;
        // private string _volume = string.Empty;
        // private string _area = string.Empty;
        // object obj = new object();

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            uiapp = commandData.Application;
            uidoc = uiapp.ActiveUIDocument;
            doc = uidoc.Document;
            object obj = new object();

            using (Transaction trans = new Transaction(doc, "xxx"))
            {
                trans.Start();
                Data data = new Data();
                FaceRecorder.Instance(doc, data).Recorder();

                var d = FaceRecorder.Instance(doc, data);
                string info = "a1" + d.Extract("a1").ToString() + "\n";
                info += "a2" + d.Extract("a2").ToString() + "\n";
                info += "a3" + d.Extract("a3").ToString() + "\n";
                TaskDialog.Show("tips", info);
                trans.Commit();
            }
            return Result.Succeeded;
        }

        ///
        ///
        public class FaceRecorder
        {
            private Document _doc = null;
            private Schema _schema = null;
            private IFaceRecorderData _data = null;
            private static FaceRecorder _instance;
            static readonly object syncRoot = new object();

            private FaceRecorder(Document doc, IFaceRecorderData recordedData)
            {
                _doc = doc;
                _data = recordedData;
            }

            public void Recorder()
            {
                SchemaBuilder builder = new SchemaBuilder(_data.Guid);
                builder.SetWriteAccessLevel(AccessLevel.Public);
                builder.SetReadAccessLevel(AccessLevel.Public);
                builder.SetSchemaName(_data.SchemaName);
                foreach (RecordData item in _data.Fields)
                {
                    if (item.Type == typeof(string) || item.Type == typeof(bool))
                    {
                        builder.AddSimpleField(item.Key, item.Type);
                    }
                    else
                    {
                        FieldBuilder fb = builder.AddSimpleField(item.Key, item.Type);
                        fb.SetUnitType(UnitType.UT_Length);
                    }
                }
                _schema = builder.Finish();
                Entity ent = new Entity(_schema);
                foreach (RecordData item in _data.Fields)
                {
                    ent.Set(item.Key, item.Value, DisplayUnitType.DUT_METERS);
                }
                //仓库
                DataStorage st = DataStorage.Create(_doc);
                st.Name = "myStorage";
                st.SetEntity(ent);
            }

            public dynamic Extract(string fieldName)
            {
                DataStorage ds = new FilteredElementCollector(_doc).OfClass(typeof(DataStorage)).Cast<DataStorage>()
                    .FirstOrDefault(m => m.Name == "myStorage");
                Schema schema = Schema.Lookup(_data.Guid);
                Type t = _data.Fields.FirstOrDefault(x => x.Key == fieldName).Type;
                Entity e = ds.GetEntity(schema);
                //?
                var o = e.GetType().GetMethod("Get", new Type[] {typeof(string), typeof(DisplayUnitType)})
                    .MakeGenericMethod(t).Invoke(e, new object[] {fieldName, DisplayUnitType.DUT_METERS});
                dynamic d = Convert.ChangeType(o, t);
                return d;
            }

            public static FaceRecorder Instance(Document doc, IFaceRecorderData recorderData)
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new FaceRecorder(doc, recorderData);
                        }
                    }
                }
                return _instance;
            }
        }

        public class Data : IFaceRecorderData
        {
            public List<RecordData> Fields
            {
                get
                {
                    return new List<RecordData>()
                    {
                        new RecordData("a1", 2.2d),
                        new RecordData("a2", true),
                        new RecordData("a3", "你好")
                    };
                }
            }
            public string SchemaName => "mySchema";
            public string StorageName => "myStorage";
            public Guid Guid => new Guid("d07f0dc5-b028-45c0-b5e7-9583353315d7");
        }

        public interface IFaceRecorderData
        {
            /// <summary>
            /// 字段字典
            /// </summary>
            List<RecordData> Fields { get; }
            /// <summary>
            /// 架构名是必须的
            /// </summary>
            string SchemaName { get; }
            /// <summary>
            /// 设置一个仓库名是必须的
            /// </summary>
            string StorageName { get; }
            /// <summary>
            /// Guid
            /// </summary>
            Guid Guid { get; }
        }

        public class RecordData
        {
            public string Key { get; set; }
            public Type Type { get; set; }
            public dynamic Value { get; set; }

            public RecordData(string key, dynamic value)
            {
                this.Key = key;
                this.Value = value;
                this.Type = value.GetType();
            }
        }
    }
}