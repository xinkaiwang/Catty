using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Catty.Bootstrap
{
    public class CancelKeyPressListener
    {
        private ManualResetEvent lockObj = new ManualResetEvent(false);

        public CancelKeyPressListener()
        {
            Console.CancelKeyPress += delegate(object sender, ConsoleCancelEventArgs e)
            {
                e.Cancel = true;
                lockObj.Set();
            };
        }

        public void WaitForEvent()
        {
            lockObj.WaitOne();
        }
    }
}
