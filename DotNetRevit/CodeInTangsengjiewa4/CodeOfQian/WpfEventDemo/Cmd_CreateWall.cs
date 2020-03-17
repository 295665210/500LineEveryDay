using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CodeInTangsengjiewa4.BinLibrary.Extensions;
using CodeInTangsengjiewa4.BinLibrary.Helpers;

namespace CodeInTangsengjiewa4.CodeOfQian.WpfEventDemo
{
    class Cmd_CreateWall : IExternalEventHandler
    {
        //属性传值
        public double WallHeight { get; set; }

        public void Execute(UIApplication uiapp)
        {
            Document doc = uiapp.ActiveUIDocument.Document;
            double height = WallHeight.MmToFeet();
            double offset = 1000d.MmToFeet();
            Curve curve = Line.CreateBound(new XYZ(10, 10, 0), new XYZ(0, 0, 0));
            ElementId wallTypeId = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls)
                .OfClass(typeof(WallType)).Cast<WallType>().OrderBy(x => x.Name).FirstOrDefault()?.Id;
            ElementId levelId = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Levels)
                .OfClass(typeof(Level)).Cast<Level>().OrderBy(x => x.Elevation).FirstOrDefault()?.Id;
            //?. 运算符: 如果前面是null,这不进行后面的操作,直接返回null;
            doc.Invoke(m => { Wall.Create(doc, curve, wallTypeId, levelId, height, offset, false, false); },
                       "create wall use eventHandler");
        }

        public string GetName()
        {
            return "Cmd_CreateWall";
        }
    }
}