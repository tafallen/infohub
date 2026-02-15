using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace uk.me.timallen.infohub
{
    public class GetNews
    {
        private readonly INewsService _newsService;
        private readonly ILogger _logger;

        public GetNews(INewsService newsService, ILoggerFactory loggerFactory)
        {
            _newsService = newsService;
            _logger = loggerFactory.CreateLogger<GetNews>();
        }

        [Function("GetNews")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequestData req)
        {
            var response = await _newsService.GetNewsAsync();
            _logger.LogInformation(response);

            var httpResponse = req.CreateResponse(HttpStatusCode.OK);
            httpResponse.Headers.Add("Content-Type", "application/json; charset=utf-8");
            httpResponse.WriteString(response);
            return httpResponse;
        }
    }
}
