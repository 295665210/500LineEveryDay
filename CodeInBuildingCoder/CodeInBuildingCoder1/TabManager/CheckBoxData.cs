using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInBuildingCoder1.TabManager
{
    public class CheckBoxData
    {
        public CheckBoxData(bool isCheck, string name)
        {
            this.IsSelected = isCheck;
            this.CheckName = name;
        }


        public bool IsSelected { get; set; }


        public string CheckName { get; set; }
    }
}