using System;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace AppInsightsQueueVsBus
{
    public class SQDoSomething
    {
        private static readonly string key = Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY", EnvironmentVariableTarget.Process);
        private readonly TelemetryClient telemetryClient;

        public SQDoSomething()
        {
            this.telemetryClient = new TelemetryClient()
            {
                InstrumentationKey = key
            };
        }

        [FunctionName("SQDoSomething")]
        public void Run([QueueTrigger("myqueue-items", Connection = "AzureWebJobsStorage")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
