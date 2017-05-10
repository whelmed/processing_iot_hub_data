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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTHubEventProcessor
{
    public class Config
    {
        /// <summary>
        /// the ConnectionString
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// the consumer group
        /// </summary>
        public string ConsumerGroupName { get; internal set; }
        /// <summary>
        /// the endpoint path
        /// </summary>
        public string Path { get; internal set; }
        public string StorageConnectionString { get; internal set; }
        public string LeaseContainerName { get; internal set; }

        public static Config Read()
        {
            var filename = $"{typeof(Config).Assembly.GetName().Name}.exe.json";
            if (!File.Exists(filename)) return new IoTHubEventProcessor.Config();
            var json = File.ReadAllText(filename);
            var config = JsonConvert.DeserializeObject<Config>(json);
            return config;
        }

        public void Write()
        {
            var json = JsonConvert.SerializeObject(this);
            var filename = $"{typeof(Config).Assembly.GetName().Name}.exe.json";
            File.WriteAllText(filename, json);
        }
    }
}
