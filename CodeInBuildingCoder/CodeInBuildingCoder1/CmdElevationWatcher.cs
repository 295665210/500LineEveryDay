using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.DB.Macros;
using Autodesk.Revit.UI;

namespace CodeInBuildingCoder1
{
    #region ElevationWatcher using DocumentChanged event
    /// <summary>
    /// React to elevation view creation subscribing to DocumentChanged event.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    class CmdElevationWatcher : IExternalCommand
    {
        /// <summary>
        /// Keep a reference to the handler, so wo know
        /// Whether we have already registered and need
        /// to unRegister or vice versa. //vice versa：反之亦然。
        /// </summary>
        static EventHandler<DocumentChangedEventArgs> _handler = null;

        /// <summary>
        /// Return the first elevation view found in the
        /// Given element id collection or null.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        static View FindElevationView(Document doc, ICollection<ElementId> ids)
        {
            View view = null;
            foreach (ElementId id in ids)
            {
                view = doc.GetElement(id) as View;
                //Creating a new view template in Revit 2013.
                //erroneously triggers the elevation trigger.
                if (view.IsTemplate && ViewType.Internal == view.ViewType)
                {
                    view = null;
                    continue;
                }
                if (null != view && ViewType.Elevation == view.ViewType)
                {
                    break;
                }
                view = null;
            }
            return view;
        }

        /// <summary>
        /// DocumentChanged envent handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void OnDocumentChanged(object sender, DocumentChangedEventArgs e)
        {
            Document doc = e.GetDocument();
            //To avoid reacting to family import,
            //ignore family documents:
            if (doc.IsFamilyDocument)
            {
                View view = FindElevationView(doc, e.GetAddedElementIds());
                if (null != view)
                {
                    string msg = string.Format(
                                               "You just created an"
                                               + "elevation view '{0}'.Are you"
                                               + "sure you want to do that?"
                                               + "(Elevations don't show hidden line"
                                               + "detail, which makes them unsuitable"
                                               + "for core wall elevations etc.", view.Name);
                    TaskDialog.Show("ElevationChecker", msg);
                }
            }
        }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;

            if (null == _handler)
            {
                _handler = new EventHandler<DocumentChangedEventArgs>(OnDocumentChanged);
                //Subscribe to DocumentChanged event
                app.DocumentChanged += _handler;
            }
            else
            {
                app.DocumentChanged -= _handler;
                _handler = null;
            }
            return Result.Succeeded;
        }
    }
    #endregion

    #region ElevationWather using DMU updater
    /// <summary>
    /// React to elevation view creation using DMU updater.
    /// </summary>
    [Transaction(TransactionMode.ReadOnly)]
    class CmdElevationWatcherUpdater : IExternalCommand
    {
        /// <summary>
        /// 
        /// </summary>
        private static ElevationWatcherUpdater _updater = null;

        public class ElevationWatcherUpdater : IUpdater
        {
            private static AddInId _appId;
            private static UpdaterId _updaterId;

            public ElevationWatcherUpdater(AddInId id)
            {
                _appId = id;
                _updaterId = new UpdaterId(_appId, new Guid("fafbf6b2-4c06-42d4-97c1-d1b4eb593eff"));
            }

            public void Execute(UpdaterData data)
            {
                Document doc = data.GetDocument();
                Application app = doc.Application;
                foreach (ElementId id in data.GetAddedElementIds())
                {
                    View view = doc.GetElement(id) as View;
                    if (null != view && ViewType.Elevation == view.ViewType)
                    {
                        TaskDialog.Show("ElevationWacher Updater", string.Format("New elevation view '{0}'",
                                         view.Name));
                    }
                }
            }

            public UpdaterId GetUpdaterId()
            {
                return _updaterId;
            }

            public ChangePriority GetChangePriority()
            {
                return ChangePriority.FloorsRoofsStructuralWalls;
            }

            public string GetUpdaterName()
            {
                return "ElevationWatcherUpdater";
            }

            public string GetAdditionalInformation()
            {
                return "The Builiding Coder," + "http://thebuildingcoder.typepad.com"; //广告无处不在。
            }
        }


        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Application app = uiapp.Application;
            if (null == _updater)
            {
                _updater = new ElevationWatcherUpdater(app.ActiveAddInId);
                //Register updater to react to view creation
                UpdaterRegistry.RegisterUpdater(_updater);
                ElementCategoryFilter f = new ElementCategoryFilter(BuiltInCategory.OST_Views);
                UpdaterRegistry.AddTrigger(_updater.GetUpdaterId(), f, Element.GetChangeTypeElementAddition());
            }
            else
            {
                UpdaterRegistry.UnregisterUpdater(_updater.GetUpdaterId());
            }
            return Result.Succeeded;
        }
    }
    #endregion

    #region Simple Updater Sample
    // From https://forums.autodesk.com/t5/revit-api-forum/iupdater-simple-example-needed/m-p/9893248
    [Transaction(TransactionMode.Manual)]
    class CmdSimpleUpdaterExample : IExternalCommand
    {
        class SimpleUpdater : IUpdater
        {
            const string Id = "d42d28af-d2cd-4f07-8873-e7cfb61903d8";
            private UpdaterId _updater_id { get; set; }

            public SimpleUpdater(Document doc, AddInId addInId)
            {
                _updater_id = new UpdaterId(addInId, new Guid(Id));
                RegisterUpdater(doc);
                RegisterTriggers();
            }

            private void RegisterUpdater(Document doc)
            {
                if (UpdaterRegistry.IsUpdaterRegistered(
                                                        _updater_id, doc))
                {
                    UpdaterRegistry.RemoveAllTriggers(_updater_id);
                    UpdaterRegistry.UnregisterUpdater(_updater_id, doc);
                }
                UpdaterRegistry.RegisterUpdater(this, doc);
            }

            private void RegisterTriggers()
            {
                if (null != _updater_id
                    && UpdaterRegistry.IsUpdaterRegistered(
                                                           _updater_id))
                {
                    UpdaterRegistry.RemoveAllTriggers(_updater_id);
                    UpdaterRegistry.AddTrigger(_updater_id,
                                               new ElementCategoryFilter(BuiltInCategory.OST_Walls),
                                               Element.GetChangeTypeAny());
                }
            }


            public void Execute(UpdaterData data)
            {
                Document doc = data.GetDocument();
                ICollection<ElementId> ids = data.GetModifiedElementIds();

                IEnumerable<string> names = ids.Select<ElementId, String>(id => doc.GetElement(id).Name);

                TaskDialog.Show("Simple Updater", string.Join<string>(",", names));
            }

            public UpdaterId GetUpdaterId()
            {
                return _updater_id;
            }

            public ChangePriority GetChangePriority()
            {
                return ChangePriority.MEPFixtures;
            }

            public string GetUpdaterName()
            {
                return "SimpleUpdater";
            }

            public string GetAdditionalInformation()
            {
                return "NA";
            }
        }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Document doc = uiapp.ActiveUIDocument.Document;
            AddInId addInId = uiapp.ActiveAddInId;

            SimpleUpdater su = new SimpleUpdater(doc, addInId);
            return Result.Succeeded;
        }
    }
    #endregion
}