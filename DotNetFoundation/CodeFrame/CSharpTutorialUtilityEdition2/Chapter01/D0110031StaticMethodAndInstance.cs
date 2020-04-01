using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSharpTutorialUtilityEdition2.Chapter01
{
    /*
     * 1.10.3 静态方法和实例方法      
     */
    class D0110031
    {
        static void Main(string[] args)
        {
            UseMethod m =new UseMethod();
            UseMethod.StaticMethod();
            m.NoStaticMethod();

        }
    }

    public class UseMethod
    {
        private static int x; //静态字段
        private int y; //实例字段

        public static void StaticMethod()
        {
            x = 10;
          //  y = 20;  静态方法不能放访问实例数据成员
        }
        public void NoStaticMethod()
        {
            x = 10; //实例方法可以反问静态数据成员
            y = 20; //实例方法可以访问实例数据成员
        }
    }
   

}