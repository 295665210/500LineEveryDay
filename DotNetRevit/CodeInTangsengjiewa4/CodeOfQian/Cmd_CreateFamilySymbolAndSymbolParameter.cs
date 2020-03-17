using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CodeInTangsengjiewa4.CodeOfQian
{
    /// <summary>
    /// create Family Symbol and symbol parameter
    /// code from:https://www.jianshu.com/p/80833193d73b
    /// </summary>
    public class Cmd_CreateFamilySymbolAndSymbolParameter : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            FamilyTypesParameter(doc);
            return Result.Succeeded;
        }

        public void FamilyTypesParameter(Document doc)
        {
            if (!doc.IsFamilyDocument)
            {
                return;
            }
            using (Transaction ts = new Transaction(doc, "family test"))
            {
                ts.Start();
                FamilyManager mgr = doc.FamilyManager;
                FamilyParameter param =
                    mgr.AddParameter("New Parameter", BuiltInParameterGroup.PG_DATA, ParameterType.Text,
                                     false); //这里是告诉set方法的参数
                for (int i = 0; i < 5; i++)
                {
                    FamilyType newType = mgr.NewType(i.ToString());
                    mgr.CurrentType = newType;
                    mgr.Set(param, "this value" + i);
                }
                ts.Commit();
            }
        }
    }
}