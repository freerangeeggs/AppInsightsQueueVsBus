using System;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace AppInsightsQueueVsBus
{
    public class SBDoSomething
    {
        private static readonly string key = Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY", EnvironmentVariableTarget.Process);
        private readonly TelemetryClient telemetryClient;

        public SBDoSomething()
        {
            this.telemetryClient = new TelemetryClient()
            {
                InstrumentationKey = key
            };
        }

        [FunctionName("SBDoSomething")]
        public void Run([ServiceBusTrigger("myqueue", Connection = "ServiceBusConnection")]string myQueueItem)
        {
            this.telemetryClient.TrackEvent($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
        }
    }
}
