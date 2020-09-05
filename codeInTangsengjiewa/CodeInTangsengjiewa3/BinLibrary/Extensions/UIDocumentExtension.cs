using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;

namespace CodeInTangsengjiewa3.BinLibrary.Extensions
{
    public static class UIDocumentExtension
    {
        public static UIView ActiveUIView(this UIDocument uidoc)
        {
            var result = default(UIView);
            var doc = uidoc.Document;
            var acview = doc.ActiveView;

            var uiviews = uidoc.GetOpenUIViews();
            result = uiviews.FirstOrDefault(m => m.ViewId == acview.Id);
            return result;
        }
    }
}