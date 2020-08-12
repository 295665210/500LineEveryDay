using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace CreateFilters
{
    public class Tools
    {
        public static List<Level> GetAllLevels(Document doc)
        {
            List<Level> levelList = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Levels).OfClass(typeof(Level)).Cast<Level>().OrderBy(x => x.Elevation).ToList();
            // List<Level> levelList = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Levels).OfClass(typeof(Level)).ToList().ConvertAll(x => x as Level);
            return levelList;
        }

        public static List<View> GetAllViews(Document doc)
        {
            List<View> allViews = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(View)).Cast<View>().ToList();
            return allViews;
        }
        public static List<FloorType> GetAllFloorTypes(Document doc)
        {
            List<FloorType> allFloorTypes = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Floors).OfClass(typeof(FloorType)).Cast<FloorType>().ToList();
            return allFloorTypes;
        }
        public static List<Element> GetAllElements(Document doc)
        {
            List<Element> allElements = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).UnionWith(new FilteredElementCollector(doc).OfClass(typeof(HostObject))).
                UnionWith(new FilteredElementCollector(doc).WhereElementIsElementType().OfCategory(BuiltInCategory.OST_Stairs)).
                UnionWith(new FilteredElementCollector(doc).WhereElementIsElementType().
                    OfCategory(BuiltInCategory.OST_StairsRailing)).UnionWith(new FilteredElementCollector(doc).WhereElementIsElementType().OfCategory(BuiltInCategory.OST_Ramps)).ToList();
            return allElements;
        }
    }
}
