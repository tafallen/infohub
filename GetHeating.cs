using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace uk.me.timallen.infohub
{
    public static class GetHeating
    {
        [FunctionName("GetHeating")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            var state = HiveHeating.GetHeatingState();
            var result = state.ToString();
            log.LogInformation(result);
            return new OkObjectResult(result);
        }
    }
}
