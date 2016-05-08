/*=======================
 * 类名：Program
 * 描述：主程序
 * 
 * 作者：chaonan99
 * 创建日期：2015/12/20
 * 最后修改：2015/12/20
 ========================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreciseNumber
{
    class Program
    {
        static void Main(string[] args)
        {

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            PreciseNumber result = new PreciseNumber();
            TimeSpan timeSpan;

            string input1;
            while (true)
            {
                Console.WriteLine("请选择方法：[1]泰勒展开法;[2]复化辛普森公式;[3]龙贝格算法：");
                input1 = Console.ReadLine();
                if (input1 != "1" && input1 != "2" && input1 != "3")
                {
                    Console.WriteLine("非法的输入！请重新输入！");
                }
                else
                {
                    break;
                }
            }
            Console.WriteLine("请输入自变量：");
            string input2 = Console.ReadLine();
            PreciseNumber x = new PreciseNumber(input2);
            Console.WriteLine("请输入精度（需要保留的小数点后位数）：");
            string input3 = Console.ReadLine();
            int decPrecise = int.Parse(input3);
            switch (input1)
            {
                case "1":
                    stopwatch.Start();
                    result = ArctanMethod.ArcTan1(x, decPrecise);
                    timeSpan = stopwatch.Elapsed;
                    break;
                case "2":
                    stopwatch.Start();
                    result = ArctanMethod.ArcTan2(x, decPrecise);
                    timeSpan = stopwatch.Elapsed;
                    break;
                case "3":
                    stopwatch.Start();
                    result = ArctanMethod.ArcTan3(x, decPrecise);
                    timeSpan = stopwatch.Elapsed;
                    break;
                default:
                    throw new Exception("程序不应该到达之处");
            }
            Console.WriteLine("结果：" + result.ToString());
            Console.WriteLine("时间：" + timeSpan.ToString());
        }
    }
}
