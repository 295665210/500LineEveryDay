using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;


namespace CodeInSDK.GetSetDefaultTypes
{
    /// <summary>
    /// CurvedBeamMainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DefaultFamilyTypes : Page, IDockablePaneProvider
    {
        public static DockablePaneId PaneId = new DockablePaneId(new Guid("{DF0F08C3-447C-4615-B9B9-4843D821012E}"));

        public DefaultFamilyTypes()
        {
            InitializeComponent();

            _handler = new DefaultFamilyTypeCommandHandler();
            _event = ExternalEvent.Create(_handler);
        }

        /// <summary>
        /// Sets document to the default family type pane.
        /// </summary>
        /// <param name="document"></param>
        public void SetDocument(Document document)
        {
            if (_document == document)
            {
                return;
            }
            _document = document;
            _dataGrid_DefaultFamilyTypes.Items.Clear();

            List<int> categories = GetAllFamilyCateogries(_document);

            if (categories.Count < 1)
            {
                return;
            }

            foreach (int cid in categories)
            {
                FamilyTypeRecord record = new FamilyTypeRecord();
                record.CategoryName = Enum.GetName(typeof(BuiltInCategory), cid);

                FilteredElementCollector collector = new FilteredElementCollector(_document);
                collector = collector.OfClass(typeof(FamilySymbol));
                var query = from FamilySymbol et in collector
                            where et.IsValidDefaultFamilyType(new ElementId(cid))
                            select et;
                ElementId defaultFamilyTypeId = _document.GetDefaultFamilyTypeId(new ElementId(cid));

                List<DefaultFamilyTypeCandidate> defaultFamilyTypeCandidates = new List<DefaultFamilyTypeCandidate>();

                foreach (FamilySymbol t in query)
                {
                    DefaultFamilyTypeCandidate item = new DefaultFamilyTypeCandidate()
                    {
                        Name = t.FamilyName + " - " + t.Name,
                        Id = t.Id,
                        CateogryId = new ElementId(cid)
                    };
                    defaultFamilyTypeCandidates.Add(item);
                    if (t.Id.IntegerValue == defaultFamilyTypeId.IntegerValue)
                    {
                        record.DefaultFamilyType = item;
                    }
                }
                record.DefaultFamilyTypeCandidates = defaultFamilyTypeCandidates;
                int index = _dataGrid_DefaultFamilyTypes.Items.Add(record);
            }
        }

        private List<int> GetAllFamilyCateogries(Document document)
        {
            FilteredElementCollector collector = new FilteredElementCollector(document);
            collector = collector.OfClass(typeof(Family));
            var query = collector.ToElements();

            List<int> categoryids = new List<int>();

            //The corresponding UI for OST_MatchModel is "Architecture->Build->Component"
            categoryids.Add((int) BuiltInCategory.OST_MatchModel);

            //The corresponding UI for OSt_MatchModel is "Annotate -> Detail -> Component"
            categoryids.Add((int) BuiltInCategory.OST_MatchDetail);

            foreach (Family t in query)
            {
                if (!categoryids.Contains(t.FamilyCategory.Id.IntegerValue))
                {
                    categoryids.Add(t.FamilyCategory.Id.IntegerValue);
                }
            }
            return categoryids;
        }


        public void SetupDockablePane(DockablePaneProviderData data)
        {
            data.FrameworkElement = this as FrameworkElement;
            data.InitialState = new DockablePaneState();
            data.InitialState.DockPosition = DockPosition.Top;
        }


        private ExternalEvent _event = null;
        private DefaultFamilyTypeCommandHandler _handler;
        private Document _document;


        private void DefaultFamilyTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1 && e.RemovedItems.Count == 1)
            {
                System.Windows.Controls.ComboBox cb = sender as System.Windows.Controls.ComboBox;
                if (cb == null)
                    return;

                DefaultFamilyTypeCandidate item = e.AddedItems[0] as DefaultFamilyTypeCandidate;
                if (item == null)
                    return;

                _handler.SetData(item.CateogryId, item.Id);
                _event.Raise();
            }


        }
    }



    public class DefaultFamilyTypeCandidate
    {
        /// <summary>
        /// The name.
        /// </summary>
        public String Name
        {
            get;
            set;
        }

        /// <summary>
        /// The element id.
        /// </summary>
        public ElementId Id
        {
            get;
            set;
        }

        /// <summary>
        /// The category id.
        /// </summary>
        public ElementId CateogryId
        {
            get;
            set;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class FamilyTypeRecord
    {
        /// <summary>
        /// The category name.
        /// </summary>
        public String CategoryName
        {
            get;
            set;
        }

        /// <summary>
        /// List of default family type candidates.
        /// </summary>
        public List<DefaultFamilyTypeCandidate> DefaultFamilyTypeCandidates
        {
            get;
            set;
        }

        /// <summary>
        /// The current default family type.
        /// </summary>
        public DefaultFamilyTypeCandidate DefaultFamilyType
        {
            get;
            set;
        }
    }

    public class DefaultFamilyTypeCommandHandler : IExternalEventHandler
    {
        ElementId _builtInCategory;
        ElementId _defaultTypeId;

        public void SetData(ElementId categoryId, ElementId typeId)
        {
            _builtInCategory = categoryId;
            _defaultTypeId = typeId;
        }

        public string GetName()
        {
            return "Reset Default family type";
        }


        public void Execute(Autodesk.Revit.UI.UIApplication revitApp)
        {
            using (Transaction tran = new Transaction(revitApp.ActiveUIDocument.Document,
                "Set Default family type to " + _defaultTypeId.ToString()))
            {
                tran.Start();
                revitApp.ActiveUIDocument.Document.SetDefaultFamilyTypeId(_builtInCategory, _defaultTypeId);
                tran.Commit();
            }
        }
    } // class CommandHandler
}