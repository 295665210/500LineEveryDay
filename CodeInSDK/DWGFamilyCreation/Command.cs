using System;
using System.Collections.Generic;
using System.IO;
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

namespace DWGFamilyCreation
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class Command : IExternalCommand
    {
        //revit application
        private UIApplication m_app;
        // revit document
        private Document m_doc;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                m_app = commandData.Application;
                m_doc = commandData.Application.ActiveUIDocument.Document;
                if (null == m_doc)
                {
                    message = "There is no active document.";
                    return Result.Failed;
                }

                if (!m_doc.IsFamilyDocument)
                {
                    message = "Current document is not a family document.";
                    return Result.Failed;
                }

                //get the view where the dwg file will be imported
                View view = GetView();
                if (null == view)
                {
                    message = "Opened wrong template file, please use the provided family template file.";
                    return Result.Failed;

                }

                //The dwg file which will be imported.
                string AssemblyDirectory =
                    Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string DwgFile = "Desk.dwg";
                string dwgFullPath = Path.Combine(AssemblyDirectory, DwgFile);


                Transaction transaction = new Transaction(m_doc, "DwgFamilyCreation");
                transaction.Start();
                //important the dwg file into current family document.
                DWGImportOptions options = new DWGImportOptions();
                options.Placement = ImportPlacement.Origin;
                options.OrientToView = true;
                ElementId elementId = null;
                m_doc.Import(dwgFullPath, options, view, out elementId);

                //add type parameters to the family
                AddParameters(DwgFile);

                transaction.Commit();


            }
            catch (Exception ex)
            {
                message = ex.ToString();
                return Result.Failed;
            }

            return Result.Succeeded;
        }

        //添加族属性
        //加的是类型属性。 属性的分组是其他。
        private void AddParameters(string DWGFileName)
        {
            //get the family manager
            FamilyManager familyManager = m_doc.FamilyManager;

            //add parameter 1:DwgFileName
            familyManager.NewType("DWGFamilyCreation");
            FamilyParameter paraFileName = familyManager.AddParameter("DWGFileName", BuiltInParameterGroup.INVALID,
                ParameterType.Text, false);
            familyManager.Set(paraFileName,DWGFileName);

            //add parameter2 : ImportTime
            string time = DateTime.Now.ToString("yyyy-mm-dd");
            FamilyParameter paraImportTime = familyManager.AddParameter("ImportTime", BuiltInParameterGroup.INVALID,
                ParameterType.Text, false);
            familyManager.Set(paraImportTime,time);

        }

        private View GetView()
        {
            View view = null;
            List<Element> views = new List<Element>();
            FilteredElementCollector collector = new FilteredElementCollector(m_app.ActiveUIDocument.Document);
            views.AddRange(collector.OfClass(typeof(View)).ToElements());
            foreach (View v in views)
            {
                // 优先级顺序
                // 无论是从上而下，还是从左到右，都是越上和越前面，优先级越高。
                //
                // 第一级：++、--(做为前缀)、()、+、-(做为单元运算符时)、!、~。
                //
                // 第二级：*、/、%、+、-。
                //
                // 第三级：<<、>>位移运算。
                //
                // 第四级：<、>、<=、>=、==、!=。（逻辑比较）
                //
                // 第五级：&、^、|、&&、||。
                //
                // 第六级：=、*=、/=、%=、+=、-=、<<=、>>=、&=、^=、|=。
                //
                // 第七级：++、--(做为后缀)。
                if (!v.IsTemplate && v.ViewType == Autodesk.Revit.DB.ViewType.FloorPlan && v.Name == "Ref.Level")
                {
                    view = v;
                    break;
                }
            }
            return view;
        }
    }
}