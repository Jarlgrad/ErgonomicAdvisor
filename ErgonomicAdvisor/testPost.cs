using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace ErgonomicAdvisor
{
    public static class testPost
    {
        [FunctionName("testPost")]
        public static async Task<string> Run
            (
                [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]Gifsercise gifsercise,
                TraceWriter log
            )
        {
            var gifRepo = new GifRepository();

            var newGifIndex = gifRepo.GetGifCount()+1;

            var gifserciseEntity = new GifserciseEntity("gif", newGifIndex.ToString());
            gifserciseEntity.image_url= gifsercise.image_url;
            gifserciseEntity.text = gifsercise.text;

            var insertResult = await gifRepo.AddGifsercise(gifserciseEntity);

            if (insertResult.HttpStatusCode < 300)
            {
                //var slackMessage = new SlackMessage($"Successfully addded |||| { gifsercise.image_url } ||||  { gifsercise.text} |||| with statusCode: { insertResult.HttpStatusCode} |||| new gifCount: { rowResult}.");
                //var ergoAdvice = JsonConvert.SerializeObject(slackMessage), Encoding.UTF8, "application/json");
                //var slackMessage = new SlackMessage("Gifsercise successfully added", );
                
                //SlackIntegration.PostToSlack(gifserciseEntity);

                var rowResult = gifRepo.UpdateRowCount(newGifIndex);
                log.Info($"C# HTTP trigger function processed a successful request. new gifCount: {rowResult}.");
                return $"Successfully addded |||| {gifsercise.image_url} ||||  {gifsercise.text} |||| with statusCode: {insertResult.HttpStatusCode} |||| new gifCount: {rowResult}.";
            }
            else
            {
                log.Info($"C# HTTP trigger function failed a request with result {insertResult.Result.ToString()}");
                return $"Failed to add new Gifsercise {insertResult.Result.ToString()}";
            }
        }
    }
}
