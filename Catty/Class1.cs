using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty
{
    public class Class1
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Class1));

        public static String Hello = "Hello";

        public void TestMethod()
        {
            log.Debug("this is a Debug message");
            log.Info("this is a Info message");
            log.Warn("this is a Warn message");
            log.Error("this is a error message");
            log.Fatal("this is a Fatal message");
        }
    }
}
