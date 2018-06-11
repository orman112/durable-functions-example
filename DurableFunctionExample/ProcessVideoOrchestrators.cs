using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace DurableFunctionExample
{
    public static class ProcessVideoOrchestrators
    {
        [FunctionName("O_ProcessVideo")]
        public static async Task<object> ProcessVideo(
            [OrchestrationTrigger] DurableOrchestrationContext ctx,
            TraceWriter log)
        {
            var videoLocation = ctx.GetInput<string>();

            var transcodedLocation = await ctx.CallActivityAsync<string>("A_TranscodeVideo", videoLocation);

            var thumbnailLocation = await ctx.CallActivityAsync<string>("A_ExtractThumbnail", transcodedLocation);

            var withIntoLocation = await ctx.CallActivityAsync<string>("A_PrependIntro", transcodedLocation);

            return new
            {
                Transcoded = transcodedLocation,
                Thumbnail = thumbnailLocation,
                WithIntro = withIntoLocation
            };
        }
    }
}
