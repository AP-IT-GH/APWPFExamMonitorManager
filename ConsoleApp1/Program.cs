using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {

            int a = 4;
            int b = 5;
            int c = 6;
            int d = 8;
            if (((a * b / c) + d) >= ((b * c + d) / a))
            {
                Console.WriteLine("Line 1 - a is greater to b");
                Console.WriteLine((a * b / c) + d);
            }
            else
            {
                Console.WriteLine("Line 1 - a is not greater to b");
                Console.WriteLine((b * c + d) / a);
            }
        }
    }
}
