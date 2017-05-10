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

using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IoTCommandLine.CommandLine;

namespace IoTHubDeviceClient
{
    static class ClientSimulator
    {
        [STAThread]
        static void Main(string[] args)
        {
            var config = new Config();

            // handle possible CTRL+C key press
            System.Console.CancelKeyPress += (s, e) =>
            {
                WriteLine("Writing config file...");
                config.Write();
            };

            try
            {
                // select commandName from argument list and remove
                var commandName = args[0].ToLower();
                args = args.Skip(1).ToArray();

                // parsing generic args
                config.ConnectionString = args.StringArg("connectionString", config.ConnectionString);
                config.DeviceId = args.StringArg("deviceId", config.DeviceId);

                WriteLine($"Sending to {config.ConnectionString}");

                // create the client object
                var deviceClient = DeviceClient.CreateFromConnectionString(config.ConnectionString, TransportType.Mqtt);
                deviceClient.SetMethodHandlerAsync("SwitchOn", (request, response) =>
                {
                    WriteLine("Method SwitchOn invoked");
                    return null;
                }, null).Wait();

                // run the command
                switch (commandName)
                {
                    case "telemetry":
                        WaitFor(TelemetryAsync(deviceClient, config, args));
                        break;
                    default:
                        NotSupported("Command unknown");
                        break;
                }
            }
            catch (Exception ex)
            {
                WriteLine($"{typeof(ClientSimulator).Assembly.GetName().Name}.exe telemetry|alert");
                WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                WriteLine("Writing config file...");
                config.Write();
            }
        }

        static async Task TelemetryAsync(DeviceClient deviceClient, Config config, string[] args)
        {
            WriteLine("CTRL+C to interrupt the read operation.");
            WriteLine("Press UP key to increase Data property by one");
            WriteLine("Press DOWN key to decrease Data property by one");

            var eventCount = args.IntegerArg("eventCount", 1000000);

            var data = 20;
            var i = 0;

            Task.Run(() => {
                while (i < eventCount)
                {
                    // introduce delay
                    Sleep(1000);

                    // send
                    var eventObject = new
                    {
                        DeviceId = config.DeviceId,
                        Index = i,
                        Data = data,
                        DateTime = DateTimeOffset.Now
                    };
                    SendEventAsync(deviceClient, eventObject).Wait();

                    i++;
                }
            });

            while (true)
            {
                var keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    data++;
                }
                else if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    data--;
                }
            }
        }

        private static async Task SendEventAsync<T>(DeviceClient deviceClient, T eventObject)
        {
            var eventJson = JsonConvert.SerializeObject(eventObject);
            var eventBytes = Encoding.UTF8.GetBytes(eventJson);

            try
            {
                Write($"Sending {eventJson}...");
                var message = new Message(eventBytes);
                await deviceClient.SendEventAsync(message);
                WriteLine("ok");
            }
            catch (Exception ex)
            {
                WriteLine($"failed [{ex.Message}]");
            }
        }
    }
}
