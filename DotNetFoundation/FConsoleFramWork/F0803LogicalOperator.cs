﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FConsoleMain
{
    class F0803
    {
        static void Main(string[] args)
        {

            int a = int.Parse(Console.ReadLine());
            int b = int.Parse(Console.ReadLine());

            if (a>b)
            {
                Console.WriteLine("a的值>b");
            }
            else
            {
                Console.WriteLine("b的值大于a");
            }
            Console.ReadKey();

        }
    }
}
