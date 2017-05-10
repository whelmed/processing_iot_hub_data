#r "Newtonsoft.Json"

using System;
using Newtonsoft.Json;

using Microsoft.Azure.Devices;

public class Feedback
{
    public string Command { get; set; }
    public string DeviceId { get; set; }
}

public static void Run(string myEventHubMessage, TraceWriter log)
{
    log.Info($"C# Event Hub trigger function processed a message: {myEventHubMessage}");
    if (string.IsNullOrWhiteSpace(myEventHubMessage)) return;
    var feedback = JsonConvert.DeserializeObject<Feedback>(myEventHubMessage);

    switch(feedback.Command)
    {
        case "SWITCH-ON":
            var serviceClient = ServiceClient.CreateFromConnectionString("HostName=processiot.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=rPkHPRJwThs8fub0lahpayn0ThLhCBtFecEVFHSJFAs=");
            var meth = new CloudToDeviceMethod("SwitchOn");
            serviceClient.InvokeDeviceMethodAsync(feedback.DeviceId, meth).Wait();
            break;
        default:
            throw new NotSupportedException();
    }
}