using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace CodeInTangsengjiewa4.BinLibrary.Extensions
{
    public static class TransactionExtension
    {
        public static void IgnoreFailure(this Transaction trans)
        {
            var options = trans.GetFailureHandlingOptions();
            options.SetFailuresPreprocessor(new FailureIgnore());
        }
    }

    public class FailureIgnore : IFailuresPreprocessor
    {
        public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
        {
            failuresAccessor.DeleteAllWarnings();
            // failuresAccessor.DeleteElements(failuresAccessor.e);
            return FailureProcessingResult.Continue;
        }
    }
}