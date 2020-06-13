using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace uk.me.timallen.infohub
{
    public static class CacheNews
    {
        [FunctionName("CacheNews")]
        public static void Run([TimerTrigger("* 0 */6 * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
