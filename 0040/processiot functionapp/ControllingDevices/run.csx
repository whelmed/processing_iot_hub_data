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

public static void Run(string myEventHubMessage, TraceWriter log, out string outputEventHubMessage)
{
    log.Info($"C# Event Hub trigger function processed a message: {myEventHubMessage}");

    var eventObject = JsonConvert.DeserializeObject<ClientEvent>(myEventHubMessage);
    if (eventObject.Data>24)
    {
        var feedbackObject = new {
            Command = "SWITCH-ON",
            DeviceId = eventObject.DeviceId
        };
        var feedbackJson = JsonConvert.SerializeObject(feedbackObject);
        outputEventHubMessage = feedbackJson;
    }
    else
    {
        outputEventHubMessage = "";
    }
}