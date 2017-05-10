#r "Newtonsoft.Json"

using System;
using Newtonsoft.Json;

public class ClientEvent
{
    public string DeviceId { get; set; }
    public int Index { get; set; }
    public decimal Data { get; set; }
    public DateTime DateTime { get; set; }
    public bool Alarm { get; set; }
}

public static void Run(string myEventHubMessage, TraceWriter log, out string outputDocument)
{
    log.Info($"C# Event Hub trigger function processed a message: {myEventHubMessage}");

    var clientEvent = JsonConvert.DeserializeObject<ClientEvent>(myEventHubMessage);
    if (clientEvent.Data>24) clientEvent.Alarm = true;
    else clientEvent.Alarm = false;

    outputDocument = JsonConvert.SerializeObject(clientEvent);
}