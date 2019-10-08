using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace AppInsightsQueueVsBus
{
    public static class StorageQueueTrigger
    {
        [FunctionName("StorageQueueTrigger")]
        public static void Run([QueueTrigger("myqueue-items")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
