using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMUDemo
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

        //是否开启动态更新
        public bool IsEnable { get; set; }
    }
}