using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CodeInBuildingCoder1
{
    [Transaction(TransactionMode.Manual)]
    class CmdCategories : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Application app = uiapp.Application;
            Document doc = uiapp.ActiveUIDocument.Document;
            Categories categories = doc.Settings.Categories;

            int nCategories = categories.Size;

            Debug.Print("{0} categories and their parents obtained" + " from the Categories collection.", nCategories);

            foreach (Category c in categories)
            {
                Category p = c.Parent;
                Debug.Print("{0} ({1}) , parent {2}", c.Name, c.Id.IntegerValue, (null == p ? "<None>" : p.Name));
            }

            Array bics = Enum.GetValues(typeof(BuiltInCategory));

            int nBics = bics.Length;

            Debug.Print("{0 } built-in categories and the " + " corresponding document ones :", nBics);

            Category cat;
            string s;
            List<BuiltInCategory> bics_null = new List<BuiltInCategory>();

            List<BuiltInCategory> bics_exception = new List<BuiltInCategory>();

            foreach (BuiltInCategory bic in bics)
            {
                try
                {
                    cat = categories.get_Item(bic);
                    if (null == cat)
                    {
                        bics_null.Add(bic);
                        s = "<null>";
                    }
                    else
                    {
                        s = string.Format("{0}  ( {1} ) ", cat.Name, cat.Id.IntegerValue);
                    }
                }
                catch (Exception ex)
                {
                    bics_exception.Add(bic);
                    s = ex.GetType().Name + " " + ex.Message;
                }

                Debug.Print(" {0}  --> {1}", bic.ToString(), s);
            }
            int nBicsNull = bics_null.Count;
            int nBicsException = bics_exception.Count;

            TaskDialog dlg = new TaskDialog("Hidden Built-in Categories");

            s = string.Format(
                              "{0} categories obtained from the categories collection;\r\n"
                              + "{1} built-in categies;\r\n" +
                              "{2} built-in categries retrieve null result;\r\n" +
                              "{3} built-in categories throw an exception;\r\n"
                            , nCategories, nBics, nBicsNull, nBicsException);

            Debug.Print(s);
            dlg.MainInstruction = s;

            s = bics_exception.Aggregate<BuiltInCategory, string>("开始", (a, bic) => a + "\n" + bic.ToString());

            Debug.Print(s);
            dlg.MainInstruction = s;
            dlg.Show();

            return Result.Succeeded;

            #region Built-in categories for legend components
            BuiltInCategory[] _bics_for_legend_component_with_FamilyInstance
                = new BuiltInCategory[]
                {
                    BuiltInCategory.OST_Casework,
                    BuiltInCategory.OST_Columns,
                    BuiltInCategory.OST_Doors,
                    BuiltInCategory.OST_DuctAccessory,
                    BuiltInCategory.OST_DuctFitting,
                    BuiltInCategory.OST_DuctTerminal,
                    BuiltInCategory.OST_ElectricalEquipment,
                    BuiltInCategory.OST_Furniture,
                    BuiltInCategory.OST_LightingFixtures,
                    BuiltInCategory.OST_MechanicalEquipment,
                    BuiltInCategory.OST_PipeAccessory,
                    BuiltInCategory.OST_PipeFitting,
                    BuiltInCategory.OST_Planting,
                    BuiltInCategory.OST_PlumbingFixtures,
                    BuiltInCategory.OST_SpecialityEquipment,
                    BuiltInCategory.OST_Sprinklers,
                    BuiltInCategory.OST_StructuralColumns,
                    BuiltInCategory.OST_StructuralFoundation,
                    BuiltInCategory.OST_StructuralFraming,
                    BuiltInCategory.OST_Windows
                };

            BuiltInCategory[] _bics_for_legend_component_with_SystemFamily
                = new BuiltInCategory[]
                {
                    BuiltInCategory.OST_CableTray,
                    BuiltInCategory.OST_Ceilings,
                    BuiltInCategory.OST_Conduit,
                    BuiltInCategory.OST_CurtainWallPanels,
                    BuiltInCategory.OST_DuctCurves,
                    BuiltInCategory.OST_FlexPipeCurves,
                    BuiltInCategory.OST_Floors,
                    BuiltInCategory.OST_PipeCurves,
                    BuiltInCategory.OST_RoofSoffit,
                    BuiltInCategory.OST_Roofs,
                    BuiltInCategory.OST_StackedWalls,
                    BuiltInCategory.OST_Walls
                };
            #endregion
        }
    }
}