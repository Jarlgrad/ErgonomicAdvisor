using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace ErgonomicAdvisor
{
    public static class GifserciseTimer
    {
        [FunctionName("GifserciseTimer")]
        public static void Run([TimerTrigger("0 05 9-17 * * 1-5")]TimerInfo myTimer, TraceWriter log)
        {
            var gifRepo = new GifRepository(log);

            var slackMessage = gifRepo.GetGifsercise();

            var result = PostToSlack(slackMessage);
            if ((int)result.Result.StatusCode < 300)
                log.Info($@"Successfully posted gif with url: {slackMessage.image_url}, 
                        text: '{slackMessage.text}' and statusCode: {result.Result.StatusCode}.");
            else
                log.Info($"failed to post to Slack with result {result.Result.ToString()}");
        }


        internal static async Task<HttpResponseMessage> PostToSlack(GifserciseEntity gifsercise)
        {
            var gifText = string.Concat(gifsercise.text, @" | <@techmoves>");
            var slackMessage = new SlackMessage(gifsercise.image_url, gifText);

            var slackOutput = new StringContent(JsonConvert.SerializeObject(slackMessage), Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                return await client.PostAsync((Environment.GetEnvironmentVariable("slack.webhook")), slackOutput);
            }
        }
    }
}
