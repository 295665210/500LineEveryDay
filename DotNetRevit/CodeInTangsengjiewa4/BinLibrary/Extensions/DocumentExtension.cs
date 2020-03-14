using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CodeInTangsengjiewa4.BinLibrary.Extensions
{
    public static class DocumentExtension
    {
        public static UIView ActiveUiView(this UIDocument uidoc)
        {
            var acview = uidoc.ActiveView;
            var uiViews = uidoc.GetOpenUIViews();

            var activeUiView = uiViews.FirstOrDefault(m => acview.Id == m.ViewId);
            return activeUiView;
        }
    }
}