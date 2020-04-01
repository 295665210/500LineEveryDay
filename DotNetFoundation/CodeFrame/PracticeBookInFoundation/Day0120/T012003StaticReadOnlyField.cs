using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace PracticeBook.Day0120
{
    /*
     * 练习5 :
     *     
     */
    public class T012003
    {
        static void Main(string[] args)
        {
            // Class01 class01Instance = new Class01();
            //
            // Class01._num2 = 20; // 静态公开字段可以从外部访问,修改
            // Console.WriteLine(Class01._num2);
            //
            // //Class01._num3 // 静态私有字段不能从外部访问
            // Console.WriteLine(Class01._num1);
            //
            // // Class01._num1 = 20; // 公开的静态只读字段不能从外部赋值.
            //
            // //不确定参数数量的情况
            // List<int> listInt = new List<int>() {1, 2, 3, 4, 6};
            // Class01.Display(1, 2, 3, 4, 6, 8);
            // Class01.Display2(1, 2, 3, 4, 6, 8, "one", "two", false, true, listInt);
            //
            // Console.WriteLine($"属性的get方法用lamda表达式些,可以跟三元表达式:{class01Instance.Nmu6}");
        }
    }

    public class Class01
    {
        //静态字段 公开的
        // public static int _num2 = 100;

        //静态字段 私有的
        // private static int _num3 = 100;

        public int Num3
        {
            set;
            get;
        }

        //属性的快速书方式: lamda表达式
        private static int _num4 = 100;

        public static int Num4
        {
            get => _num4;
            set => _num4 = value;
        }

        // 属性的get方法可以使用lamda表达式写.只有get方法.
        public int Nmu6 => _num4 > 100 ? 99 : 10;


        //静态只读字段
        public static readonly int _num1 = 10;

        //静态构造函数
        static Class01()
        {
            _num1 = 20; //静态只读字段可以在构造函数 初始化值.
        }

        //方法参数属性不确定的情况
        public static void Display(params int[] list)
        {
            for (int i = 0; i < list.Length; i++)
            {
                Console.WriteLine(list[i]);
            }
        }

        public static void Display2(params object[] list)
        {
            for (int i = 0; i < list.Length; i++)
            {
                Console.WriteLine(list[i]);
            }
        }
    }
}