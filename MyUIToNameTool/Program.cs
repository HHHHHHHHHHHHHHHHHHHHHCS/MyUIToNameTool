using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyUIToNameTool
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Main main = new Main();
            main.Start();


            while (Console.ReadKey().Key != ConsoleKey.Escape)
            {

            }
        }
    }
}
