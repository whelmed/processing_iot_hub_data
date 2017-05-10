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
using Microsoft.Azure.Documents.Client;

namespace IoTHubEventProcessor
{
    public class LoggingEventProcessorFactory : IEventProcessorFactory
    {
        public string DocumentDbConnectionString { get; internal set; }

        IEventProcessor IEventProcessorFactory.CreateEventProcessor(PartitionContext context)
        {
            var processor = Activator.CreateInstance<EventProcessor>();
            if (!string.IsNullOrWhiteSpace(DocumentDbConnectionString)) processor.HandleEvent = HandleEvent;
            return processor;
        }

        private void HandleEvent(ClientEvent clientEvent, PartitionContext context)
        {
            // xx.Split(new char[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries)
            // return only two strings, so it does not affects the '=' charachters inside the second value that are due to the base64 encoding of the accountkey
            //var cs = DocumentDbConnectionString.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(xx => xx.Split(new char[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries)).ToDictionary(xx => xx[0], xx => xx[1]);
            //var client = new DocumentClient(new Uri(cs["AccountEndpoint"]), cs["AccountKey"]);
            //client.CreateDocumentAsync("dbs/demo/colls/events", clientEvent).Wait();
        }
    }
}
