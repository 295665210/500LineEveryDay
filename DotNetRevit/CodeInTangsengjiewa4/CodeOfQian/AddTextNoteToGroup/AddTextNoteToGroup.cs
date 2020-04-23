using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CodeInTangsengjiewa4.CodeOfQian.AddTextNoteToGroup.UIs;
using View = Autodesk.Revit.DB.View;

namespace CodeInTangsengjiewa4.CodeOfQian.AddTextNoteToGroup
{
    [Transaction(TransactionMode.Manual)]
    public class AddTextNoteToGroup : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document document = commandData.Application.ActiveUIDocument.Document;
            View activeView = document.ActiveView;
            bool flag = !(activeView is ViewPlan);
            Result result;
            if (flag)
            {
                TaskDialog.Show("Error", "本功能只能在平面视图中使用!");
                result = Result.Cancelled;
            }
            else
            {
                List<Group> groupList = new FilteredElementCollector(document, activeView.Id)
                    .OfCategory(BuiltInCategory.OST_IOSModelGroups).OfClass(typeof(Group)).Cast<Group>()
                    .ToList<Group>();
                bool flag2 = groupList.Count == 0;
                if (flag2)
                {
                    TaskDialog.Show("Error", "此平面视图中,未检测到组!");
                    result = Result.Cancelled;
                }
                else
                {
                    List<Group> list2 = new List<Group>();
                    bool flag3 = true;
                    GroupSelectForm groupSelectForm = new GroupSelectForm(groupList, activeView);
                    bool flag4 = groupSelectForm.ShowDialog() == true;
                    if (flag4)
                    {
                        list2 = groupSelectForm.selectedGroups;
                        // flag3 = groupSelectForm.isSetGroupRed;
                        // ProgressForm progressForm = new ProgressForm(null, list2.Count);
                        // progressForm.SetNotice("正在批量生成组标签....");
                        // progressForm.Show();
                        using (Transaction transaction = new Transaction(document, "创建族标签"))
                        {
                            transaction.Start();
                            // foreach (Group group in list2)
                            // {
                            //     bool flag5 = flag3;
                            //     if (flag5)
                            //     {
                            //         FillPatternElement fillPatternElement = new FilteredElementCollector(document)
                            //             .OfClass(typeof(FillPatternElement)).Cast<FillPatternElement>().First(
                            //                 (FillPatternElement f) => f.GetFillPattern().IsSolidFill);
                            //
                            //         OverrideGraphicSettings elementOverrides = activeView.GetElementOverrides(group.Id);
                            //         elementOverrides.SetCutForegroundPatternId(fillPatternElement.Id);
                            //         elementOverrides.SetCutForegroundPatternColor(new Color(255, 0, 0));
                            //         elementOverrides.SetProjectionLineColor(new Color(255, 0, 0));
                            //         foreach (ElementId elementId in group.GetMemberIds())
                            //         {
                            //             activeView.SetElementOverrides(elementId, elementOverrides);
                            //         }
                            //     }
                            //     string name = group.Name;
                            //     TextNote textNote = TextNote.Create(document, activeView.Id,
                            //         (group.Location as LocationPoint).Point, name,
                            //         document.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType));
                            //     progressForm.AddStep();
                            // }
                            transaction.Commit();
                        }
                        result = Result.Succeeded;
                    }
                    else
                    {
                        result = Result.Cancelled;
                    }
                }
            }

            return result;
        }
    }

    internal class GroupComparer : IComparer<Group>
    {
        public int Compare(Group x, Group y)
        {
            Level level = x.Document.GetElement(x.LevelId) as Level;
            Level level2 = y.Document.GetElement(y.LevelId) as Level;
            double elevation = level.Elevation;
            double elevation2 = level2.Elevation;
            bool flag = elevation == elevation2;
            int result;
            if (flag)
            {
                string name = x.Name;
                string name2 = y.Name;
                result = name.CompareTo(name2);
            }
            else
            {
                result = elevation.CompareTo(elevation2);
            }
            return result;
        }
    }
}