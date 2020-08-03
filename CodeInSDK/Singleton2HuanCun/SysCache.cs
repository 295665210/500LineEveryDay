using System;

namespace Singleton2HuanCun
{
    public class SysCache
    {
        private SysCache()
        {
        }

        private static SysCache _instance;

        public static SysCache Instance
        {
            get
            {
                if (ReferenceEquals(_instance, null))
                {
                    _instance = new SysCache();
                }
                return _instance;
            }
        }

        public string TextValue { get; set; }

    }
}