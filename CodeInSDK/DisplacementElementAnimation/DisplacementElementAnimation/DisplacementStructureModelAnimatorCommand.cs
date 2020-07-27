using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace DisplacementElementAnimation
{
    [Transaction(TransactionMode.Manual)]
    public class DisplacementStructureModelAnimatorCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            new DisplacementStructureModelAnimator(commandData.Application, true).StartAnimation();

            return Result.Succeeded;
        }
    }
}