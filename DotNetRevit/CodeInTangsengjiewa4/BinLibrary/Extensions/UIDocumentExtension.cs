using Autodesk.Revit.UI;
using System.Linq;

namespace CodeInTangsengjiewa4.BinLibrary.Extensions
{
    public static class UIDocumentExtension
    {
        public static UIView ActiveUIView(this UIDocument uidoc)
        {
            var result = default(UIView);
            var doc = uidoc.Document;
            var acView = doc.ActiveView;

            var uiViews = uidoc.GetOpenUIViews();
            result = uiViews.FirstOrDefault(m => m.ViewId == acView.Id);
            return result;
        }
    }
}