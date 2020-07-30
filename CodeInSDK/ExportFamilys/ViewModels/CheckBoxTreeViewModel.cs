using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportFamilys.ViewModels
{
    class CheckBoxTreeViewModel:NotificationObject
    {
        public  string Tag { get; set; }
        private bool? _IsChecked = false;
        private List<CheckBoxTreeViewModel> _Children;
    }
}
