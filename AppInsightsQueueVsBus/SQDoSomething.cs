using Microsoft.ApplicationInsights;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AppInsightsQueueVsBus
{
    public class SQDoSomething
    {
        private static HttpClient httpClient;
        private static readonly string key = Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY", EnvironmentVariableTarget.Process);
        private readonly TelemetryClient telemetryClient;

        public SQDoSomething()
        {
            this.telemetryClient = new TelemetryClient()
            {
                InstrumentationKey = key
            };

            httpClient = httpClient ?? new HttpClient();
        }

        [FunctionName("SQDoSomething")]
        public async Task Run([QueueTrigger("myqueue", Connection = "AzureWebJobsStorage")] string myQueueItem)
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
