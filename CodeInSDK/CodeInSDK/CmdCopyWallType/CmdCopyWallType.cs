using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CodeInBuildingCoder
{
    [Transaction(TransactionMode.Manual)]
    class CmdCopyWallType : IExternalCommand
    {
        //根据实际情况修改
        private const string _source_project_path = "Z:/a/case/sfdc/06676034/test/NewWallType.rvt";

        private const string _wall_type_name = "NewWallType";

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            //open source project
            Document docHasFamily = app.OpenDocumentFile(_source_project_path);

            //find system family to copy, e.g. using a name wall type
            WallType wallType = null;

            FilteredElementCollector wallTypes = new FilteredElementCollector(docHasFamily).OfClass(typeof(WallType));

            int i = 0;

            foreach (WallType wt in wallTypes)
            {
                string name = wt.Name;
                Debug.Print(" {0} {1} ", ++i, name);

                if (name.Equals(_wall_type_name))
                {
                    wallType = wt;
                    break;
                }
            }

            if (null == wallType)
            {
                message = string.Format("Cannot find source wall type '{0}' " + " in source document '{1}'.",
                    _wall_type_name, _source_project_path);
                return Result.Failed;
            }

            //create a new wall type in current document.
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Transfer Wall Type");
                WallType newWallType = null;

                //
                wallTypes = new FilteredElementCollector(doc).OfClass(typeof(WallType));
                foreach (WallType wt in wallTypes)
                {
                    if (wt.Kind == wallType.Kind)
                    {
                        newWallType = wt.Duplicate(_wall_type_name) as WallType;
                        Debug.Print(string.Format("New wall type '{0}' created.", _wall_type_name));
                        break;
                    }
                }

                Parameter p = null;
                foreach (Parameter p2 in newWallType.Parameters)
                {
                    Definition d = p2.Definition;
                    if (p2.IsReadOnly)
                    {
                        Debug.Print(string.Format("Parameter '{0} is read-only. ", d.Name));
                    }
                    else
                    {
                        p = wallType.get_Parameter(d);
                        if (null == p)
                        {
                            Debug.Print(string.Format("Parameter '{0}' not found on source wall type.", d.Name));
                        }
                        else
                        {
                            if (p.StorageType == StorageType.ElementId)
                            {
                                Debug.Print(string.Format("Parameter '{0}' is an element id.", d.Name));
                            }
                            else
                            {
                                if (p.StorageType == StorageType.Double)
                                {
                                    p2.Set(p.AsDouble());
                                }
                                else if (p.StorageType == StorageType.String)
                                {
                                    p2.Set(p.AsString());
                                }
                                else if (p.StorageType == StorageType.Integer)
                                {
                                    p2.Set(p.AsInteger());
                                }
                                Debug.Print(string.Format("Parameter '{0}' copied.", d.Name));
                            }
                        }
                    }
                }
                MemberInfo[] memberInfos = newWallType.GetType().GetMembers(BindingFlags.GetProperty);
                foreach (MemberInfo m in memberInfos)
                {
                }
                t.Commit();
            }
            return Result.Succeeded;
        }
    }
}