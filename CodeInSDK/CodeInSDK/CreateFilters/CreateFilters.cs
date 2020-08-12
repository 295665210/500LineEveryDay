using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using View = Autodesk.Revit.DB.View;

namespace CreateFilters
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class CreateFilters : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            View acview = uidoc.ActiveView;

            List<View> allViews = Tools.GetAllViews(doc);
            //应该先创建系统类型，然后遍历系统类型，向过滤器加规则
            CreateFiltersMainWindow mainWindow = new CreateFiltersMainWindow();
            mainWindow.ShowDialog();

            List<string> newSystemTypes = mainWindow.stNames; //用户选择的名称
            Transaction trans = new Transaction(doc, "创建过滤器");
            trans.Start();
            List<MEPSystemType> stsList = CreateSystemTypes(doc, newSystemTypes);
            //获取当前类别所有系统类型
            FilteredElementCollector systemTypeFc = new FilteredElementCollector(doc).OfClass(typeof(MEPSystemType));

            //获取类别
            List<ElementId> categories = mainWindow.categoryId;
            List<FilterRule> fr = new List<FilterRule>();
            List<List<FilterRule>> filterRules = new List<List<FilterRule>>();

            ElementId paraId = mainWindow.paraId;

            for (int i = 0; i < stsList.Count; i++)
            {
                // ????????????
                fr.Add(ParameterFilterRuleFactory.CreateEqualsRule(paraId, stsList[i].Id));
                filterRules.Add(fr);
            }

            List<ParameterFilterElement> parameterFilterElements = CreateParameterFilters(doc, newSystemTypes, categories);
            trans.Commit();
            return Result.Succeeded;
        }
        /// <summary>
        /// 创建参数过滤器
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="filtersName"></param>
        /// <param name="elIds"></param>
        /// <returns></returns>
        public List<ParameterFilterElement> CreateParameterFilters(Document doc, List<string> filtersName, ICollection<ElementId> elIds)
        {

            List<ParameterFilterElement> paraFiltersList = new List<ParameterFilterElement>();
            for (int i = 0; i < filtersName.Count; i++)
            {
                paraFiltersList.Add(ParameterFilterElement.Create(doc, filtersName[i], elIds));
            }

            return paraFiltersList;
        }

        public List<MEPSystemType> CreateSystemTypes(Document doc, List<string> inputName)
        {
            List<Element> stsList = new FilteredElementCollector(doc).OfClass(typeof(MEPSystemType)).ToList();
            List<string> stsName = stsList.ConvertAll(x => x.Name);
            List<string> newSystemNames = inputName.Except(stsName).ToList(); //区原有系统类型和新类型的差值
            List<MEPSystemType> mEPSystemTypes = new List<MEPSystemType>();

            if (stsList.Count != 0)
            {
                for (int i = 0; i < newSystemNames.Count; i++)
                {
                    mEPSystemTypes.Add(((MEPSystemType)stsList.First()).Duplicate(newSystemNames[i]) as MEPSystemType);
                }
            }
            return mEPSystemTypes;
        }
    }
}