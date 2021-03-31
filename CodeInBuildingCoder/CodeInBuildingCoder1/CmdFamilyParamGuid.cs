using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CodeInBuildingCoder1
{
    [Transaction(TransactionMode.ReadOnly)]
    class CmdFamilyParamGuid : IExternalCommand
    {
        bool GetFamilyParamGuid(FamilyParameter fp, out string guid)
        {
            guid = string.Empty;
            bool isShared = false;
            System.Reflection.FieldInfo fi = fp.GetType()
                .GetField("m_Parameter",
                          System.Reflection.BindingFlags.Instance
                          | System.Reflection.BindingFlags.NonPublic);
            if (null != fi)
            {
                Parameter p = fi.GetValue(fp) as Parameter;
                isShared = p.IsShared;
                if (isShared && null != p.GUID)
                {
                    guid = p.GUID.ToString();
                }
            }
            return isShared;
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
                return Result.Failed;
            }
            else
            {
                bool isShared;
                string guid;
                FamilyManager mgr = doc.FamilyManager;
                foreach (FamilyParameter fp in mgr.Parameters)
                {
                    isShared = GetFamilyParamGuid(fp, out guid);
                    //using extension method,internally
                    //accessing getParameter;
                    if (fp.IsShared())
                    {
                        Guid guid2 = fp.GUID;
                    }
                }
                return Result.Succeeded;
            }
        }
    }
}