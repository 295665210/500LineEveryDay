using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace FamilyManagerUI
{
    class FamilyObject
    {
        public string Name { get; set; } //族名
        public string Location { get; set; } //族路径
        public BitmapSource RfaImage { get; set; } //族预览图片
    }
}
