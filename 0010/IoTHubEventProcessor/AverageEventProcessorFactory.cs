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

using Microsoft.Azure.EventHubs.Processor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;

using static IoTCommandLine.CommandLine;
using Newtonsoft.Json;
using System.IO;

namespace IoTHubEventProcessor
{
    public class AverageEventProcessorFactory : IEventProcessorFactory
    {
        public string EventHubConnectionString { get; internal set; }

        IEventProcessor IEventProcessorFactory.CreateEventProcessor(PartitionContext context)
        {
            var processor = Activator.CreateInstance<EventProcessor>();
            processor.HandleEvent = HandleEvent;
            return processor;
        }

        private struct Sample
        {
            public DateTime Date { get; set; }
            public decimal Data { get; set; }
            public string PartitionId { get; internal set; }
        }

        private Queue<Sample> _queue = new Queue<Sample>();

        private void HandleEvent(ClientEvent clientEvent, PartitionContext context)
        {

            // enqueue the event
            _queue.Enqueue(new Sample { Date = DateTime.Now, Data = clientEvent.Data, PartitionId = context.PartitionId });

            while (true)
            {
                // if empty, exit
                if (_queue.Count == 0) break;

                // if more that 10, remove oldest and continue
                if (_queue.Count > 10)
                {
                    _queue.Dequeue();
                    continue;
                }

                // if the top one (the oldest) is more that 10 seconds old, remove oldest and continue;
                if ((DateTime.Now - _queue.Peek().Date).Seconds > 10)
                {
                    _queue.Dequeue();
                    continue;
                }

                // exit, because there are no more that 10 events not older that 10 seconds.
                break;
            }

            // calculate the average of current messages
            var average = _queue.Average(xx => xx.Data);

            // if there are at least 8 messages with an average of 24,....
            if (average > 24 && _queue.Count >= 8)
            {
                // send the message
                var eventHubClient = EventHubClient.CreateFromConnectionString(EventHubConnectionString);
                var eventJson = JsonConvert.SerializeObject(new {
                    Command = "SWITCH-ON",
                    DeviceId = clientEvent.DeviceId
                });
                WriteLine($"Sending feedback: {eventJson}");
                byte[] eventBytes = Encoding.UTF8.GetBytes(eventJson);
                eventHubClient.SendAsync(new EventData(eventBytes)).Wait();
            }
        }
    }
}
