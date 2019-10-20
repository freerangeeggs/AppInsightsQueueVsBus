using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AppInsightsQueueVsBus
{
    public class HttpSendToServiceBus
    {
        private static HttpClient httpClient;
        private static readonly string key = Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY", EnvironmentVariableTarget.Process);
        private readonly TelemetryClient telemetryClient;

        public HttpSendToServiceBus()
        {
            this.telemetryClient = new TelemetryClient()
            {
                InstrumentationKey = key
            };

            httpClient = httpClient ?? new HttpClient();
        }

        [FunctionName("HttpSendToServiceBus")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [ServiceBus("myqueue", Connection = "ServiceBusConnection", EntityType = EntityType.Queue)] ICollector<string> queueCollector)
        {
            this.telemetryClient.TrackEvent("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            if (name == null)
            { 
                return new BadRequestObjectResult("Please pass a name on the query string or in the request body");
            }

            HttpResponseMessage response = await httpClient.GetAsync($"https://swapi.co/api/people/?search={name}");
            PeopleSearchResultCollection result = JsonConvert.DeserializeObject<PeopleSearchResultCollection>(await response.Content.ReadAsStringAsync());

            if (result.count == 0)
            {
                return new BadRequestObjectResult("Nobody found");
            }

            foreach (Person character in result.results)
            {
                queueCollector.Add(JsonConvert.SerializeObject(character));
            }

            return new OkObjectResult($"{result.count} results have been sent to Service Bus Queue.");
        }
    }
}
