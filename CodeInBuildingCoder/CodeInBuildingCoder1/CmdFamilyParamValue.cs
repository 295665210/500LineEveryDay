using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CodeInBuildingCoder1
{
    [Transaction(TransactionMode.Manual)]
    class CmdFamilyParamValue : IExternalCommand
    {
        #region SetFamilyParameterValue
        void SetFamilyParameterValueWorks(Document doc, string paramNameToAmend)
        {
            FamilyManager mgr = doc.FamilyManager;
            FamilyParameter familyParam = mgr.get_Parameter(paramNameToAmend);
            if (familyParam != null)
            {
                foreach (FamilyType familyType in mgr.Types)
                {
                    Debug.Print(familyType.Name);
                    mgr.CurrentType = familyType;
                    mgr.Set(familyParam, 2);
                }
            }
        }
        #endregion

        static string FmailyParamValueString(
            FamilyType t, FamilyParameter fp, Document doc)
        {
            string value = t.AsValueString(fp);
            switch (fp.StorageType)
            {
                case StorageType.Double:
                    value = Util.RealString((double) t.AsDouble(fp))
                            + "(double)";
                    break;
                case StorageType.ElementId:
                    ElementId id = t.AsElementId(fp);
                    Element e = doc.GetElement(id);
                    value = id.IntegerValue.ToString() + "("
                            + Util.ElementDescription(e) + ")";
                    break;
                case StorageType.Integer:
                    value = t.AsInteger(fp).ToString() + "(int)";
                    break;
                case StorageType.String:
                    value = "'" + t.AsString(fp) + "'(string)";
                    break;
            }
            return value;
        }

        public Result Execute(
            ExternalCommandData commandData, ref string message,
            ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Document doc = uiapp.ActiveUIDocument.Document;
            if (!doc.IsFamilyDocument)
            {
                message = "Please run this command in a family document.";
            }
            else
            {
                FamilyManager mgr = doc.FamilyManager;
                int n = mgr.Parameters.Size;
                Debug.Print("\nFamily {0} has {1} parameter {2}.", doc.Title, n,
                            Util.PluralSuffix(n));

                Dictionary<string, FamilyParameter> fps =
                    new Dictionary<string, FamilyParameter>(n);
                foreach (FamilyParameter fp in mgr.Parameters)
                {
                    string name = fp.Definition.Name;
                    fps.Add(name, fp);
                }

                List<string> keys = new List<string>(fps.Keys);
                keys.Sort();
                n = mgr.Types.Size;
                Debug.Print("Family {0} has {1} type{2}{3}", doc.Title, n,
                            Util.PluralSuffix(n), Util.DotOrColon(n));

                foreach (FamilyType t in mgr.Types)
                {
                    string name = t.Name;
                    Debug.Print(" {0}:", name);
                    foreach (string key in keys)
                    {
                        FamilyParameter fp = fps[key];
                        if (t.HasValue(fp))
                        {
                            string value = FmailyParamValueString(t, fp, doc);
                            Debug.Print("   {0} = {1}", key, value);
                        }
                    }
                }
            }

            #region Exercise ExtractPartAtomFromFamilyFile
            //here is a completely different way to get
            //all the parameter values,and all the other
            //family information as well;
            bool exercise_this_method = false;
            if (doc.IsFamilyDocument && exercise_this_method)
            {
                string path = doc.PathName;
                if (0 < path.Length)
                {
                    uiapp.Application
                        .ExtractPartAtomFromFamilyFile(path, path + ".xml");
                }
            }
            #endregion //Exercise ExtractPartAtomFromFamilyFile

            return Result.Failed;
        }
    }
}