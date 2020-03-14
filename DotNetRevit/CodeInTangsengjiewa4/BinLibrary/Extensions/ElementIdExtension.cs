using Autodesk.Revit.DB;

namespace CodeInTangsengjiewa4.BinLibrary.Extensions
{
    public static class ElementIdExtension
    {
        public static Element GetElement(this ElementId elementId, Document doc)
        {
            return doc.GetElement(elementId);
        }
    }
}