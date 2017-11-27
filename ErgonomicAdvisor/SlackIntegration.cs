using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;

namespace ErgonomicAdvisor
{
    public static class SlackIntegration
    {
        [FunctionName("SlackIntegration")]
        public static void Run([TimerTrigger("0 26 9-17 * * 1-5")]TimerInfo myTimer, TraceWriter log)
        {
            var gifRepo = new GifRepository();

            var slackMessage = gifRepo.GetGifsercise();

            PostToSlack(slackMessage);

            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
        }


        internal static void PostToSlack(GifserciseEntity gifsercise)
        {
            var slackMessage = new SlackMessage(gifsercise.image_url, gifsercise.text);

            var ergoAdvice = new StringContent(JsonConvert.SerializeObject(slackMessage), Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                var result = client.PostAsync((Environment.GetEnvironmentVariable("slack.webhook")), ergoAdvice);

                Console.WriteLine(client.BaseAddress + "T124A62R2/B80KR3XC0/IlXKCl9tXN2WsJVOe04T4ntA");
                Console.WriteLine(ergoAdvice.ReadAsStringAsync().Result);
                Console.WriteLine(result.Result.RequestMessage);
                Console.WriteLine(result.Result);
                Console.WriteLine(result.Status);
                Console.WriteLine(result.CreationOptions);
                Console.WriteLine(result.IsCompleted);

                Console.ReadLine();
            }
        }
    }
}
