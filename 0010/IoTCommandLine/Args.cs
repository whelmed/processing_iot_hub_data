/*
    MIT License

    Copyright (c) 2017 Marco Parenzan and Cloud Academy

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IoTCommandLine
{
    public static partial class CommandLine
    {
        /// <summary>
        /// Find for a generic parameter -{name} to be converted to Enum 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args">The argument list</param>
        /// <param name="name">the name of the argument</param>
        /// <param name="defaultValue">default value if string is empty</param>
        /// <returns></returns>
        public static T? EnumArg<T>(this string[] args, string name, T? defaultValue = null)
            where T: struct
        {
            if (!typeof(T).IsEnum) InvalidOperation($"{typeof(T).Name} is not valid type");

            var stringValue = FindArg(args, name);
            if (string.IsNullOrWhiteSpace(stringValue)) return defaultValue;

            return (T?) Enum.Parse(typeof(T), stringValue);
        }

        /// <summary>
        /// Find for a generic parameter -{name} to be converted to double 
        /// </summary>
        /// <param name="args">The argument list</param>
        /// <param name="name">the name of the argument</param>
        /// <param name="defaultValue">default value if string is empty</param>
        /// <returns></returns>
        public static double? DoubleArg(this string[] args, string name, double? defaultValue = null)
        {
            var stringValue = FindArg(args, name);
            if (string.IsNullOrWhiteSpace(stringValue)) return defaultValue;

            return double.Parse(stringValue);
        }


        /// <summary>
        /// Find for a generic parameter -{name} to be converted to int 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args">The argument list</param>
        /// <param name="name">the name of the argument</param>
        /// <param name="defaultValue">default value if string is empty</param>
        /// <returns></returns>
        public static int? IntegerArg(this string[] args, string name, int? defaultValue = null)
        {
            var stringValue = FindArg(args, name);
            if (string.IsNullOrWhiteSpace(stringValue)) return defaultValue;

            return int.Parse(stringValue);
        }

        /// <summary>
        /// Find for a generic parameter -{name} to be converted to boolean 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args">The argument list</param>
        /// <param name="name">the name of the argument</param>
        /// <param name="defaultValue">default value if string is empty</param>
        /// <returns></returns>
        public static bool? BoolArg(this string[] args, string name, bool? defaultValue = null)
        {
            var stringValue = FindArg(args, name);
            if (string.IsNullOrWhiteSpace(stringValue)) return defaultValue;

            return bool.Parse(stringValue);
        }

        /// <summary>
        /// Find for a generic parameter -{name} to be converted to boolean 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args">The argument list</param>
        /// <param name="name">the name of the argument</param>
        /// <param name="defaultValue">default value if string is empty</param>
        /// <returns></returns>
        public static bool ClipboardArg(this string[] args)
        {
            return args.ExistArg("clipboard") && args.BoolArg("clipboard", true).Value;
        }

        /// <summary>
        /// Find for a generic parameter -{name} to be converted to string 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args">The argument list</param>
        /// <param name="name">the name of the argument</param>
        /// <param name="defaultValue">default value if string is empty</param>
        /// <returns></returns>
        public static string StringArg(this string[] args, string name, string defaultValue = null)
        {
            var stringValue = FindArg(args, name);
            if (string.IsNullOrWhiteSpace(stringValue)) return defaultValue;

            return stringValue;
        }

        /// <summary>
        /// Find for a generic parameter -{name} to be converted to generic type T 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args">The argument list</param>
        /// <param name="name">the name of the argument</param>
        /// <param name="separator">characther separating values in string representation</param>
        /// <param name="defaultValue">default value if string is empty</param>
        /// <returns></returns>
        public static T[] ArrayArg<T>(this string[] args, string name, char separator = ',', T[] defaultValue = null)
        {
            var stringValue = FindArg(args, name);
            if (string.IsNullOrWhiteSpace(stringValue)) return defaultValue;

            if (typeof(T) == typeof(string))
                return stringValue.Split(separator).Cast<T>().ToArray();
            else
                return stringValue.Split(separator).Select(xx => Convert.ChangeType(xx, typeof(T))).Cast<T>().ToArray();
        }

        /// <summary>
        /// Find for a generic parameter -{name} to be converted to string
        /// Overload for different default parameter distribution
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args">The argument list</param>
        /// <param name="name">the name of the argument</param>
        /// <param name="separator">characther separating values in string representation</param>
        /// <param name="defaultValue">default value if string is empty</param>
        /// <returns></returns>
        public static string[] ArrayArg(this string[] args, string name, string[] defaultValue = null, char separator = ',')
        {
            return ArrayArg(args, name, separator, defaultValue);
        }

        /// <summary>
        /// Find for a generic parameter -{name} to be converted to string 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args">The argument list</param>
        /// <param name="name">the name of the argument</param>
        /// <returns></returns>
        public static bool ExistArg(this string[] args, string name)
        {
            var stringValue = FindArg(args, name);
            return stringValue != null;
        }

        private static string FindArg(string[] args, string name)
        {
            var argumentPrefix = $"-{ name}";
            var found = args.Where(xx => xx.StartsWith(argumentPrefix, StringComparison.InvariantCultureIgnoreCase)).ToList();
            if (found.Count == 0) return null;
            if (found.Count > 1) Argument($"Too many arguments {name}");

            return found[0].Substring(argumentPrefix.Length);
        }

        private static string[] FindArgs(string[] args, string name)
        {
            var argumentPrefix = $"-{ name}";
            var arg = args.Where(xx => xx.StartsWith(argumentPrefix, StringComparison.InvariantCultureIgnoreCase)).ToList();
            if (args.Length == 0) return new string[] { };

            return args.Select(xx => xx.Substring(argumentPrefix.Length)).ToArray();
        }
    }
}
