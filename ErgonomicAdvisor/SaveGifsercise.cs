using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Net.Http;

namespace ErgonomicAdvisor
{
    public static class SaveGifsercise
    {
        [FunctionName("Gifsercise")]
        public static async Task<string> Run
            (
                [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequestMessage req,
                TraceWriter log
            )
        {
            try
            {
                var gifRepo = new GifRepository(log);

                var gif = CreateEntity(req, gifRepo);
                log.Info($"In testPost: new Entity: index: {gif.RowKey}, image_url: {gif.image_url}, text: {gif.text}");

                var insertResult = await gifRepo.AddGifsercise(gif);

                if (insertResult.HttpStatusCode < 300)
                {
                    var rowResult = gifRepo.UpdateRowCount();
                    log.Info($"Successful put gifsercise. new max index: {rowResult}.");

                    return $"Successfully addded gif with index: {rowResult}, url: {gif.image_url}, text: '{gif.text}' and statusCode: {insertResult.HttpStatusCode}.";
                }
                else
                {
                    log.Info($"failed a put request with result {insertResult.Result.ToString()}");
                    return $"Failed to add new Gifsercise {insertResult.Result.ToString()}";
                }
            }
            catch (System.Exception ex)
            {
                log.Error($"insert unsuccessful", ex);
                return $"Insert unsuccessful {ex.Message}, {ex.StackTrace}";
            }
        }

        private static GifserciseEntity CreateEntity(HttpRequestMessage req, GifRepository repo)
        {
            var newGifIndex = repo.GetGifCount() + 1;

            var valuePairs = req.Content.ReadAsFormDataAsync().Result;

            return new GifserciseEntity(
                    partition : "gif",
                    row: newGifIndex.ToString(), 
                    url : valuePairs["url"], 
                    text : valuePairs["text"]
            );
        }
    }
}
