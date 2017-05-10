using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IoTCommandLine
{
    public static partial class CommandLine
    {
        public static void WaitFor(Task t)
        {
            t.Wait();
        }
    }
}
