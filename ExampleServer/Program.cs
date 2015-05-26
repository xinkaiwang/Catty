using Catty;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BasicConfigurator.Configure();
            Console.WriteLine(Catty.Class1.Hello);
            new Class1().TestMethod();
        }
    }
}
