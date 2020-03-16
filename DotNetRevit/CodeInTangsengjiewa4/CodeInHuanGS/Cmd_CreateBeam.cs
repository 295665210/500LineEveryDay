using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using CodeInTangsengjiewa4.BinLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeInTangsengjiewa4.CodeInHuanGS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_CreateBeam : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var app = uiapp.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;

            Transaction ts = new Transaction(doc, "创建梁");
            ts.Start();
            //get the active view's level for beam creation
            Level level = doc.ActiveView.GenLevel;
            //load a family symbol from file
            FamilySymbol gotSymbol = null;
            string fileName = @"C:\ProgramData\Autodesk\RVT 2020\Libraries\China\结构\框架\钢\堞形梁.rfa";
            string fileNameFromFilePath = System.IO.Path.GetFileNameWithoutExtension(fileName);
            string SymbolName = "CB460X28.3";
            FamilyInstance instance = null;
            if (doc.LoadFamilySymbol(fileName, SymbolName, out gotSymbol))
            {
                gotSymbol.Activate();
                //look for a model in the list of selected elements
                ICollection<Element> eles = sel.GetElementIds().Select(m => m.GetElement(doc)).ToList();
                ModelLine modelLine = null;
                foreach (Element element in eles)
                {
                    if (element is ModelLine)
                    {
                        modelLine = element as ModelLine;
                    }
                }
                if (null != modelLine)
                {
                    //create new beam
                    instance = doc.Create.NewFamilyInstance(modelLine.GeometryCurve, gotSymbol, level,
                                                            StructuralType.Beam);
                }
            }
            else
            {
                throw new Exception("could not load" + fileName);
            }
            ts.Commit();
            return Result.Succeeded;
        }
    }
}