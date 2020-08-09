using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QiShiLog;
using QiShiLog.Log;

namespace TryCatch
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.Instance.EnableConsole = true;
            Logger.Instance.EnableInfoFile = true;

            //显示内容
            Logger.Instance.Info("日志1");
            Logger.Instance.Info("测试1");

            for (int i = 0; i < 5; i++)
            {
                try
                {
                    int result = 10 / i;
                    Console.WriteLine(result);
                }
                catch (Exception e)
                {

                    Logger.Instance.Info($"{e.Message},此时i为{i}");
                    Process.Start(Path.Combine(QiShiCore.WorkSpace.Dir, "Log"));
                    // Console.WriteLine(e.Message);
                    // Console.WriteLine($"{e.Message},此时i为{i}");

                    // throw; //抛出异常，中断程序

                    //通常汇报出错的信息写在日志里。
                }

                //不管程序对或者错，下面这句话都要运行
                finally
                {
                    Console.WriteLine("不论如何，都会运行这句。");
                }
            }
            Process.Start(Path.Combine(QiShiCore.WorkSpace.Dir, "Log"));
            Console.ReadKey();
        }
    }
}