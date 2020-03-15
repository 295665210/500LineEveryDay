using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System;


namespace CodeInTangsengjiewa4.BinLibrary.Extensions
{
    /// <summary>
    /// 看得有点糊涂
    /// </summary>
    public class SelectionFilter : ISelectionFilter
    {
        private Document _doc;
        private Type _type;

        public SelectionFilter(Document doc, Type type)
        {
            _doc = doc;
            _type = type;
        }

        public bool AllowElement(Element elem)
        {
            if (elem.GetType() == _type)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return true;
        }
    }

    public class MultiSelectionFilter : ISelectionFilter
    {
        private Func<Element, bool> eleFunc;
        private Func<Reference, bool> refFunc;

        public MultiSelectionFilter(Func<Element, bool> func1, Func<Reference, bool> func2)
        {
            eleFunc = func1;
            refFunc = func2;
        }

        public bool AllowElement(Element elem)
        {
            return (refFunc != null) ? true : eleFunc(elem);
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return refFunc == null ? false : refFunc(reference);
        }
    }

    public static class SelectionFilterHelper
    {
        public static MultiSelectionFilter GetSelectionFilter(
            this Document doc, Func<Element, bool> func1, Func<Reference, bool> func2 = null)
        {
            return new MultiSelectionFilter(func1, func2);
        }
    }
}