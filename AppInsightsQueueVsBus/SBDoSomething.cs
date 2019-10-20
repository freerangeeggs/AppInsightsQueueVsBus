using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AppInsightsQueueVsBus
{
    public class SBDoSomething
    {
        private static HttpClient httpClient;
        private static readonly string key = Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY", EnvironmentVariableTarget.Process);
        private readonly TelemetryClient telemetryClient;

        public SBDoSomething()
        {
            this.telemetryClient = new TelemetryClient()
            {
                InstrumentationKey = key
            };

            httpClient = httpClient ?? new HttpClient();
        }

        [FunctionName("SBDoSomething")]
        public async Task Run([ServiceBusTrigger("myqueue", Connection = "ServiceBusConnection")]string myQueueItem)
        {
            Person character = JsonConvert.DeserializeObject<Person>(myQueueItem);

            this.telemetryClient.TrackEvent($"Looking up films for: {character.name}");
            Console.WriteLine($"Looking up films for: {character.name}");

            foreach (string film in character.films)
            {
                this.telemetryClient.TrackTrace($"{character.name}: {film}");
                Console.WriteLine($"{character.name}: {film}");
                await httpClient.GetAsync(film);
            }
        }
    }
}
