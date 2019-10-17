using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

namespace AppInsightsQueueVsBus
{
    public class HttpSendToQueue
    {
        private static readonly string key = Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY", EnvironmentVariableTarget.Process);
        private readonly TelemetryClient telemetryClient;

        public HttpSendToQueue()
        {
            this.telemetryClient = new TelemetryClient
            {
                InstrumentationKey = key
            };
        }

        [FunctionName("HttpSendToQueue")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req)
        {
            this.telemetryClient.TrackEvent("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            using (StreamReader sr = new StreamReader(req.Body))
            {
                string requestBody = await sr.ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                name = name ?? data?.name;
            }

            return name != null
                ? (ActionResult)new OkObjectResult($"Hello, {name}")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
