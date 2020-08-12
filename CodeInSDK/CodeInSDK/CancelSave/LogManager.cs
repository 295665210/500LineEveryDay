using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;

namespace CancelSave
{
    // one log file will be created by this class for tracking events raise.
    public class LogManager
    {
        // a trace listener for the output log of CancelSave sample
        private static TraceListener TxtListener;
        // the directory where this assembly in.
        private static string AssemblyLocation =
            Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        //retrieval if doing regression test now.
        //if the ExpectedOutPut.log exists in the assembly folder returns true,else returns false.
        public static bool RegressionTestNow
        {
            get
            {
                string expectedLogFile = Path.Combine(AssemblyLocation, "ExpectedOutPut.log");
                if (File.Exists(expectedLogFile))
                {
                    return true;
                }
                return false;
            }
        }

        //Static constructor which creates a log file.
        static LogManager()
        {
            //create CancelSave.log
            string actullyLogFile = Path.Combine(AssemblyLocation, "CancelSave.log");
            //if already existed,delete it
            if (File.Exists(actullyLogFile))
            {
                File.Delete(actullyLogFile);
            }
            TxtListener = new TextWriterTraceListener(actullyLogFile);
            Trace.Listeners.Add(TxtListener);
            Trace.AutoFlush = true;
        }

        //finalize and close the output log.
        public static void LogFinalize()
        {
            Trace.Flush();
            TxtListener.Close();
            Trace.Close();
            Trace.Listeners.Remove(TxtListener);
        }

        //write log to file:which event occurred in which document
        public static void WriteLog(EventArgs args, Document doc)
        {
            Trace.WriteLine("");
            Trace.WriteLine("[Event]" + GetEventName(args.GetType()) + ":" + TitleNoExt(doc.Title));
        }

        public static void WriteLog(string message)
        {
            Trace.WriteLine(message);
        }

        private static string GetEventName(Type type)
        {
            string argName = type.ToString();
            string tail = "EventArgs";
            string head = "Autodesk.Revit.DB.Events.";
            int firstIndex = head.Length;
            int length = argName.Length - head.Length - tail.Length;
            string eventName = argName.Substring(firstIndex, length);
            return eventName;
        }


        private static string TitleNoExt(string orgTitle)
        {
            if (String.IsNullOrEmpty(orgTitle))
            {
                return "";
            }

            int pos = orgTitle.LastIndexOf('.');
            if (-1 != pos)
            {
                return orgTitle.Remove(pos);
            }
            else
            {
                return orgTitle;
            }
        }
    }
}