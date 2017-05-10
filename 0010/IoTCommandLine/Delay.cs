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
        static Random random = new Random();

        public static void Sleep(int value)
        {
            Thread.Sleep(value);
        }

        public static void Sleep(int min, int max)
        {
            Sleep(RandomInteger(min, max));
        }

        public static void Sleep(int? min, int? max)
        {
            Sleep(min ?? 100, max ?? 1000);
        }

        public static int RandomInteger(int min, int max)
        {
            return min + random.Next(max - min);
        }

        public static int RandomInteger(int? min, int? max)
        {
            return RandomInteger(min ?? 0, max ?? 0);
        }

        public static double RandomDouble(double min, double max)
        {
            return min + random.NextDouble()*(max-min);
        }

        public static double RandomDouble(double? min, double? max)
        {
            return RandomDouble(min ?? 0, max ?? 0);
        }
    }
}
