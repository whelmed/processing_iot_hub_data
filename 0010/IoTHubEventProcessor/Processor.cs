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

using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using static IoTCommandLine.CommandLine;

namespace IoTHubEventProcessor
{
    static class Processor
    {
        static void Main(string[] args)
        {
            var config = Config.Read();

            try
            {
                // select commandName from argument list and remove
                var commandName = args[0].ToLower();
                args = args.Skip(1).ToArray();

                // parsing generic args
                config.ConnectionString = args.StringArg("connectionString", AppSetting("connectionString"));
                config.ConsumerGroupName = args.StringArg("consumerGroupName", PartitionReceiver.DefaultConsumerGroupName);
                config.Path = args.StringArg("path", args.StringArg("path", AppSetting("path")));
                config.StorageConnectionString = args.StringArg("storageConnectionString", AppSetting("storageConnectionString"));
                config.LeaseContainerName = args.StringArg("leaseContainerName", commandName);
                
                var eventProcessorHost = new EventProcessorHost(
                                 config.Path,
                                 config.ConsumerGroupName,
                                 config.ConnectionString,
                                 config.StorageConnectionString,
                                 config.LeaseContainerName);

                WriteLine($"Receiving from {config.ConnectionString}");
                WriteLine($"Path {config.Path}");
                WriteLine($"Consumer Group Name {config.ConsumerGroupName}");

                // handle possible CTRL+C key press
                System.Console.CancelKeyPress += (s, e) =>
                {
                    // Disposes of the Event Processor Host
                    eventProcessorHost.UnregisterEventProcessorAsync().Wait();
                    WriteLine("Writing config file...");
                    config.Write();
                };

                // run the command
                switch (commandName)
                {
                    case "logging":
                        WaitFor(LoggingAsync(eventProcessorHost, config, args));
                        break;
                    case "average":
                        WaitFor(AverageAsync(eventProcessorHost, config, args));
                        break;
                    default:
                        NotSupported("Command unknown");
                        break;
                }
            }
            catch (Exception ex)
            {
                WriteLine($"{typeof(Processor).Assembly.GetName().Name}.exe telemetry|alerts");
                WriteLine($"Error: {ex.Message}");
            }
        }

        static async Task LoggingAsync(EventProcessorHost eventProcessorHost, Config config, string[] args)
        {
            //await eventProcessorHost.RegisterEventProcessorAsync<EventProcessor>();

            var factory = new LoggingEventProcessorFactory();
            factory.DocumentDbConnectionString = args.StringArg("documentDbConnectionString");
            await eventProcessorHost.RegisterEventProcessorFactoryAsync(factory);

            var clock = 0;
            Task.Run(() =>
            {
                while (true)
                {
                    Console.WriteLine($"Time:{clock++}");
                    Thread.Sleep(1000);
                }
            });

            WriteLine($"Receiving. Press enter key to stop worker.");
            ReadLine();

            // Disposes of the Event Processor Host
            await eventProcessorHost.UnregisterEventProcessorAsync();
        }

        static async Task AverageAsync(EventProcessorHost eventProcessorHost, Config config, string[] args)
        {
            var factory = new AverageEventProcessorFactory();
            factory.EventHubConnectionString = args.StringArg("eventHubConnectionString");

            await eventProcessorHost.RegisterEventProcessorFactoryAsync(factory);

            WriteLine($"Receiving. Press enter key to stop worker.");
            ReadLine();

            // Disposes of the Event Processor Host
            await eventProcessorHost.UnregisterEventProcessorAsync();
        }
    }
}
