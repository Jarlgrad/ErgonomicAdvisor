using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;
using System.Linq;

namespace ErgonomicAdvisor
{
    internal class GifRepository
    {
        private readonly CloudStorageAccount _storageAccount;
        private readonly CloudTableClient _tableClient;
        private readonly CloudTable _table;
        private readonly TraceWriter _log;

        internal GifRepository(TraceWriter log)
        {
            _storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            _tableClient = _storageAccount.CreateCloudTableClient();
            _table = _tableClient.GetTableReference(Environment.GetEnvironmentVariable("Table"));
            _log = log;
        }

        internal GifserciseEntity GetGifsercise()
        {
            var gifIndex = GetRandomGifIndex().ToString();

            var getOperation = TableOperation.Retrieve<GifserciseEntity>("gif", gifIndex);

            var operationResult = _table.Execute(getOperation);
            return (GifserciseEntity)operationResult.Result;
        }

        internal async Task<TableResult> AddGifsercise(GifserciseEntity gifsercise)
        {
            _log.Info($"In gifRepo: new Entity: index: {gifsercise.RowKey}, image_url: {gifsercise.image_url}, text: {gifsercise.text}");
            try
            {
                TableOperation insertOperation = TableOperation.Insert(gifsercise);

                var insertResult = await _table.ExecuteAsync(insertOperation);
                if (insertResult.HttpStatusCode < 300)
                    _log.Info($"insert successful with status: {insertResult.HttpStatusCode}");
                else
                    _log.Info($"insert failed with status: {insertResult.HttpStatusCode}, result: {insertResult.Result.ToString()}");

                return insertResult;
            }
            catch (Exception ex)
            {
                _log.Error($"insert unsuccessful", ex);
                throw;
            }
        }

        internal string UpdateRowCount()
        {
            var currentGifCount = GetGifCount();
            if (currentGifCount == 1)
                return AddGifCounter(new CounterEntity(currentGifCount.ToString()));
            
            DeleteGifCount(currentGifCount);
            return AddGifCounter(new CounterEntity((currentGifCount + 1).ToString()));
        }

        internal string AddGifCounter(CounterEntity gifCounter)
        {
            var insertOperation = TableOperation.InsertOrReplace(gifCounter);
            var replaceResult = _table.Execute(insertOperation);
            return gifCounter.RowKey.ToString();
        }
        private void DeleteGifCount(int gifCount)
        {
            var deleteCount = new CounterEntity(gifCount.ToString());
            TableOperation deleteOperation = TableOperation.Delete(deleteCount);
            var deleteResult = _table.Execute(deleteOperation);
        }

        internal int GetRandomGifIndex()
        {
            int count = GetGifCount();

            var random = new Random();
            return random.Next(2, count);
        }

        internal int GetGifCount()
        {
            var query = new TableQuery<CounterEntity>()
                       .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "count"));

            var count = _table.ExecuteQuery(query)
                        .FirstOrDefault()
                        .RowKey;

            return int.Parse(count);
        }
    }
}
