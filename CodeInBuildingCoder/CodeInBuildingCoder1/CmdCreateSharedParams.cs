using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CodeInBuildingCoder1
{
    class CmdCreateSharedParams
    {
        private const string _filename = "C:/tmp/SharedParams.txt";
        private const string _groupName = "The Building Coder Parameters";
        private const string _defName = "SP";
        private ParameterType _defType = ParameterType.Number;

        private BuiltInCategory[] targets = new BuiltInCategory[]
        {
            BuiltInCategory.OST_Doors,
            BuiltInCategory.OST_Walls,
            BuiltInCategory.OST_IOSModelGroups,
            BuiltInCategory.OST_Lines
        };

        Category GetCategory(Document doc, BuiltInCategory target)
        {
            Category cat = null;
            if (target.Equals(BuiltInCategory.OST_IOSModelGroups))
            {
                FilteredElementCollector collector =
                    Util.GetElementsOfType(doc, typeof(Group),
                                           BuiltInCategory.OST_IOSModelGroups);
                IList<Element> modelGroups = collector.ToElements();
                if (0 == modelGroups.Count)
                {
                    Util.ErrorMsg("Please insert a model group.");
                    return cat;
                }
                else
                {
                    cat = modelGroups[0].Category;
                }
            }
            else
            {
                try
                {
                    cat = doc.Settings.Categories.get_Item(target);
                }
                catch (Exception exception)
                {
                    Util.ErrorMsg($"Error obtaining document {target.ToString()} category : {exception.Message}");
                }
            }
            return cat;
        }


        bool CreateSharedParameter(
            Document doc, Category cat, int nameSuffix, bool typeParameter)
        {
            Application app = doc.Application;
            Autodesk.Revit.Creation.Application ca = app.Create;

            //get or set the current shared param fileName;
            string filename = app.SharedParametersFilename;

            if (0 == filename.Length)
            {
                string path = _filename;
                StreamWriter stream;
                stream = new StreamWriter(path);
                stream.Close();
                app.SharedParametersFilename = path;
                filename = app.SharedParametersFilename;
            }
            //get the current shared params file object:
            DefinitionFile file = app.OpenSharedParameterFile();
            if (null == file)
            {
                Util.ErrorMsg("Error getting the shared params file.");
                return false;
            }

            //get or create the shared params group:
            DefinitionGroup group = file.Groups.get_Item(_groupName);
            if (null == group)
            {
                group = file.Groups.Create(_groupName);
            }

            if (null == group)
            {
                Util.ErrorMsg("Error getting the shared params group.");
                return false;
            }

            bool visible = cat.AllowsBoundParameters;
            //get or create the shared params definition:
            string defname = _defName + nameSuffix.ToString();

            Definition definition = group.Definitions.get_Item(defname);

            if (null == definition)
            {
                ExternalDefinitionCreationOptions opt =
                    new ExternalDefinitionCreationOptions(defname, _defType);
                opt.Visible = visible;
                definition = group.Definitions.Create(opt);
            }

            if (null == definition)
            {
                Util.ErrorMsg("Error creating shared parameter.");
                return false;
            }

            //create the category set containing our category for binding;
            CategorySet catSet = ca.NewCategorySet();
            catSet.Insert(cat);

            try
            {
                Binding binding = typeParameter
                                      ? ca.NewTypeBinding(catSet) as Binding
                                      : ca.NewInstanceBinding(catSet) as
                                            Binding;

                // we could check if it is already bound,
                // but it looks like insert will just ignore
                // it in that case:

                doc.ParameterBindings.Insert(definition, binding);

                // we can also specify the parameter group here:

                //doc.ParameterBindings.Insert( definition, binding,
                //  BuiltInParameterGroup.PG_GEOMETRY );

                Debug.Print(
                            "Created a shared {0} parameter '{1}' for the {2} category.",
                            (typeParameter ? "type" : "instance"),
                            defname, cat.Name);
            }
            catch (Exception ex)
            {
                Util.ErrorMsg(string.Format(
                                            "Error binding shared parameter to category {0}: {1}",
                                            cat.Name, ex.Message));
                return false;
            }

            return true;
        }

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIApplication app = commandData.Application;
            Document doc = app.ActiveUIDocument.Document;

            using (Transaction t = new Transaction(doc))
            {
                t.Start("Create Shared Parameter");
                Category cat;
                int i = 0;

                // create instance parameters:

                foreach (BuiltInCategory target in targets)
                {
                    cat = GetCategory(doc, target);
                    if (null != cat)
                    {
                        CreateSharedParameter(doc, cat, ++i, false);
                    }
                }

                // create a type parameter:

                cat = GetCategory(doc, BuiltInCategory.OST_Walls);
                CreateSharedParameter(doc, cat, ++i, true);
                t.Commit();
            }
            return Result.Succeeded;
        }


        #region Modify Many shared  Parameter Values
        class IdForSynchro
        {
            public ElementId RevitId { get; set; }
            public int Param1 { get; set; }
            public string Param2 { get; set; }
            public double Param3 { get; set; }
        }


        void modifyParameterValues(Document doc, IList<IdForSynchro> data)
        {
            using (Transaction tr = new Transaction(doc))
            {
                Guid guid1 = Guid.Empty;
                Guid guid2 = Guid.Empty;
                Guid guid3 = Guid.Empty;
                Guid guid4 = Guid.Empty;
                tr.Start("synchro");

                foreach (IdForSynchro d in data
                ) //main.idForSynchro s the collection of data
                {
                    Element e = doc.GetElement(d.RevitId);
                    if (Guid.Empty == guid1)
                    {
                        guid1 = e.LookupParameter("PLUGIN_PARAM1").GUID;
                        guid1 = e.LookupParameter("PLUGIN_PARAM2").GUID;
                        guid1 = e.LookupParameter("PLUGIN_PARAM3").GUID;
                    }
                    e.get_Parameter(guid1).Set(d.Param1);
                    e.get_Parameter(guid2).Set(d.Param2);
                    e.get_Parameter(guid3).Set(d.Param3);
                }
                tr.Commit();
            }
        }
        #endregion

        #region SetAllowVaryBetweenGroups
        /// <summary>
        /// Helper method to control ‘SetAllowVaryBetweenGroups’
        /// option for instance binding param
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="guid"></param>
        /// <param name="AllowVaryBetweenGroups"></param>
        static void SetInstanceParamVaryBetweenGroupsBehaviour(
            Document doc, Guid guid, bool AllowVaryBetweenGroups = true)
        {
            try //last resort
            {
                SharedParameterElement sp =
                    SharedParameterElement.Lookup(doc, guid);
                //Should never happen as we will call 
                //this only for *existing* shared param
                if (null == sp)
                {
                    return;
                }

                InternalDefinition def = sp.GetDefinition();
                if (def.VariesAcrossGroups != AllowVaryBetweenGroups)
                {
                    //must be within an outer transaction!
                    def.SetAllowVaryBetweenGroups(doc, AllowVaryBetweenGroups);
                }
            }
            catch
            {
            } //ideally ,should report something to log...
        }
        #endregion
    }
}