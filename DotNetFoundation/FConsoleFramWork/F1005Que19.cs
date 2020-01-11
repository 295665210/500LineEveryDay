﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FConsoleMain
{
 class F1005
    {
        /// <summary>
        /// 题目19
        /// 使用判断语句,计算折扣率问题.
        /// 如果顾客消费满100元,9折;
        /// 如果顾客消费满300元,8折;
        /// 如果消费满500元, 7折.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            int cost = int.Parse(Console.ReadLine());

            int count;

            if (cost >= 500)
            {
                count = 9;
            }
            else if (cost < 500 && cost >= 300)
            {
                count = 8;
            }
            else if (cost < 300 && cost >= 100)
            {
                count = 9;
            }
            else
            {
                count = 10;
            }

            Console.WriteLine("消费了{0}元,可以打{1}折",cost,count);
            Console.ReadKey();

        }
    }
}