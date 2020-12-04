using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CodeInTangsengjiewa3.BinLibrary.Helpers;
using Microsoft.Win32;

namespace CodeInTangsengjiewa3.通用
{
    /// <summary>
    /// 批量链接
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_MultipleLinkFile : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;

            var acview = doc.ActiveView;

            //未写完

            var opdg = new OpenFileDialog();
            opdg.Multiselect = true;
            opdg.Filter = "(*.rvt)|*.rvt";
            var dialogresult = opdg.ShowDialog();

            var count = opdg.FileNames.Length;
            string[] files = new string[count];

            if (dialogresult == true)
            {
                files = opdg.FileNames;
            }

            doc.Invoke(m =>
            {
                foreach (var file in files)
                {
                    var linktypeId = CreateRevitLink(doc, file);
                    CreateLinkInstances(doc, linktypeId);
                }
            }, "批量链接");

            return Result.Succeeded;
        }

        public ElementId CreateRevitLink(Document doc, string pathName)
        {
            FilePath path = new FilePath(pathName);
            RevitLinkOptions options = new RevitLinkOptions(false);

#if Revit2019
            LinkLoadResult result = RevitLinkType.Create(doc, path, options);
            return result.ElementId;
#endif
        }

        public void CreateLinkInstances(Document doc, ElementId linkTypeId)
        {
            RevitLinkInstance instance2 = RevitLinkInstance.Create(doc, linkTypeId);
        }
    }
}