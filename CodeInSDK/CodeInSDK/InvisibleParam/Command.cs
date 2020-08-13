using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CodeInSDK.InvisibleParam
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Transaction transaction =
                new Transaction(commandData.Application.ActiveUIDocument.Document, "External tools");
            try
            {
                transaction.Start();
                //create a clear file as parameter file
                String path = Assembly.GetExecutingAssembly().Location;
                int index = path.LastIndexOf("\\");
                String newPath = path.Substring(0, index);
                newPath += "\\RevitParameters.txt";

                if (File.Exists(newPath))
                {
                    File.Delete(newPath);
                }
                FileStream fs = File.Create(newPath);
                fs.Close();

                //cache application handle
                Application revitApp = commandData.Application.Application;

                //Prepare shared parameter file
                commandData.Application.Application.SharedParametersFilename = newPath;

                //open shared parameter file
                DefinitionFile parafile = revitApp.OpenSharedParameterFile();

                //get walls category
                Category wallCat =
                    commandData.Application.ActiveUIDocument.Document.Settings.Categories.get_Item(BuiltInCategory.OST_Walls);

                CategorySet categories = revitApp.Create.NewCategorySet();
                categories.Insert(wallCat);

                InstanceBinding binding = revitApp.Create.NewInstanceBinding(categories);

                //create a group
                DefinitionGroup apiGroup = parafile.Groups.Create("APIGroup");

                //create a visible "VisibleParam" of text type.
                ExternalDefinitionCreationOptions ExternalDefinitionCreationOptions1 =
                    new ExternalDefinitionCreationOptions("VisibleParam", ParameterType.Text);
                Definition visibleParamDef = apiGroup.Definitions.Create(ExternalDefinitionCreationOptions1);

                BindingMap bindingMap = commandData.Application.ActiveUIDocument.Document.ParameterBindings;

                bindingMap.Insert(visibleParamDef, binding);

                //create a invisible "InvisibleParam" of text type.
                ExternalDefinitionCreationOptions ExternalDefinitionCreationOptions2 =
                    new ExternalDefinitionCreationOptions("InvisibleParam", ParameterType.Text);
                Definition invisibleParamDef = apiGroup.Definitions.Create(ExternalDefinitionCreationOptions2);
                bindingMap.Insert(invisibleParamDef, binding);
            }
            catch (Exception e)
            {
                transaction.RollBack();
                message = e.ToString();
                return Result.Cancelled;
            }

            finally
            {
                transaction.Commit();
            }
            return Result.Failed;
        }
    }
}