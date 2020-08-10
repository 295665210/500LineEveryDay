using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit;
using Autodesk.Revit.DB;

namespace CodeInSDK.ExternalResourceDBServer
{
    static class KeynotesDatabase
    {
        /// <summary>
        /// Indicates what version of keynote data is currently available from the database.
        /// </summary>
        public static string CurrentVersion
        {
            //Assume that the server's keynote data is updated at the beginning of every month.
            get { return System.DateTime.Now.ToString("MM-yyyy"); }
        }

        /// <summary>
        /// Validates the specified database 'key.'
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsValidDBKey(String key)
        {
            return key == "1" || key == "2" || key == "3" || key == "4";
        }

        public static void LoadKeynoteEntries(string key, ref KeyBasedTreeEntriesLoadContent kdrlc)
        {

        }
    }
}