using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Web.Http.Results;

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
            try
            {
                var gifRepo = new GifRepository(log);

                var newGifIndex = gifRepo.GetGifCount() + 1;

                var gifserciseEntity = new GifserciseEntity("gif", newGifIndex.ToString());
                gifserciseEntity.image_url = gifsercise.image_url;
                gifserciseEntity.text = gifsercise.text;

                log.Info($"In testPost: about to add gif to Table Storage. ");
                log.Info($"In testPost: new Entity: index: {newGifIndex}, image_url: {gifserciseEntity.image_url}, text: {gifserciseEntity.text}");
                var insertResult = await gifRepo.AddGifsercise(gifserciseEntity);

                if (insertResult.HttpStatusCode < 300)
                {
                    //var slackMessage = new SlackMessage($"Successfully addded |||| { gifsercise.image_url } ||||  { gifsercise.text} |||| with statusCode: { insertResult.HttpStatusCode} |||| new gifCount: { rowResult}.");
                    //var ergoAdvice = JsonConvert.SerializeObject(slackMessage), Encoding.UTF8, "application/json");
                    //var slackMessage = new SlackMessage("Gifsercise successfully added", );

                    //SlackIntegration.PostToSlack(gifserciseEntity);

                    var rowResult = gifRepo.UpdateRowCount();
                    log.Info($"C# HTTP trigger function processed a successful request. new gif max index: {rowResult}.");
                    return $"Successfully addded |||| {gifsercise.image_url} ||||  {gifsercise.text} |||| with statusCode: {insertResult.HttpStatusCode} |||| new gif max index: {rowResult}.";
                }
                else
                {
                    log.Info($"C# HTTP trigger function failed a request with result {insertResult.Result.ToString()}");
                    return $"Failed to add new Gifsercise {insertResult.Result.ToString()}";
                }
            }
            catch (System.Exception ex)
            {
                log.Error($"insert unsuccessful", ex);
                return $"Insert unsuccessful {ex.Message}, {ex.StackTrace}";
            }
        }
    }
}
