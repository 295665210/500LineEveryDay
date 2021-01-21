using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace CodeInBuildingCoder1.LinkCadTextToModelText
{
    public class CADTextModel
    {
        public XYZ Location { get; set; }
        public string Text { get; set; }
        public double Angel { get; set; }
        public string Layer { get; set; }
    }
}