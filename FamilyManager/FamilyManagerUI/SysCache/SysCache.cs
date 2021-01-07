using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;

namespace FamilyManagerUI
{
    class SysCache
    {
        private SysCache()
        { }
        private static SysCache _instance;

        public static SysCache Instance
        {
            get
            {
                if (ReferenceEquals(null, _instance))
                {
                    _instance = new SysCache();
                                   }
                return _instance;
            }
        }

        public ExternalEvent LoadEvent { get; set; } //建立外部事件

        public string CurrentRfaLocation { get; set; } //当前族路径

    }
}
