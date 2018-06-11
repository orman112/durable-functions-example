using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace DurableFunctionExample
{
    public static class ProcessVideoStarter
    {
        [FunctionName("ProcessVideoStarter")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequestMessage req, 
            [OrchestrationClient] DurableOrchestrationClient starter,
            TraceWriter log)
        {
            string video = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "Video", true) == 0)
                .Value;

            dynamic data = await req.Content.ReadAsAsync<object>();

            video = video ?? data?.video;

            if (video == null)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest,
                    "Please pass the video location in the query string or in the request body");
            }

            log.Info($"About to start orchestration for {video}");           

            var orchestrationId = await starter.StartNewAsync("O_ProcessVideo", video);

            return starter.CreateCheckStatusResponse(req, orchestrationId);
        }
    }
}
