using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;

namespace ErgonomicAdvisor
{
    public static class GifserciseTimer
    {
        [FunctionName("GifserciseTimer")]
        public static void Run([TimerTrigger("0 * 9-17 * * 1-5")]TimerInfo myTimer, TraceWriter log)
        {
            var gifRepo = new GifRepository(log);

            var slackMessage = gifRepo.GetGifsercise();

            PostToSlack(slackMessage);

            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
        }


        internal static void PostToSlack(GifserciseEntity gifsercise)
        {
            var gifText = string.Concat(gifsercise, @"| \<@techmoves\>");
            var slackMessage = new SlackMessage(gifsercise.image_url, gifsercise.text);

            var ergoAdvice = new StringContent(JsonConvert.SerializeObject(slackMessage), Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                var result = client.PostAsync((Environment.GetEnvironmentVariable("slack.webhook")), ergoAdvice);
            }
        }
    }
}
