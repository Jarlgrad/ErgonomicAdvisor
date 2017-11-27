using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;

namespace ErgonomicAdvisor
{
    internal class GifRepository
    {
        private readonly CloudStorageAccount _storageAccount;
        private readonly CloudTableClient _tableClient;
        private readonly CloudTable _table;

        internal GifRepository()
        {
            _storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            _tableClient = _storageAccount.CreateCloudTableClient();
            _table = _tableClient.GetTableReference(Environment.GetEnvironmentVariable("Table"));
        }

        internal GifserciseEntity GetGifsercise()
        {
            var gifIndex = GetGifIndex().ToString();

            var getOperation = TableOperation.Retrieve<GifserciseEntity>("gif", gifIndex);

            var operationResult = _table.Execute(getOperation);
            return (GifserciseEntity)operationResult.Result;
        }

        internal async Task<TableResult> AddGifsercise(GifserciseEntity gifsercise)
        {
            TableOperation insertOperation = TableOperation.Insert(gifsercise);
            var insertResult = await _table.ExecuteAsync(insertOperation);

            return insertResult;
        }
        internal string UpdateRowCount(int gifCount)
        { 
            var countEntity = new CountEntity(gifCount.ToString());
            
            TableOperation replaceOperation = gifCount == 2
                ? TableOperation.InsertOrReplace(countEntity)
                : TableOperation.Replace(countEntity);

            var replaceResult = _table.Execute(replaceOperation);

            if (replaceResult.HttpStatusCode < 300)
                return countEntity.RowKey;
            else
                return replaceResult.HttpStatusCode.ToString();
        }

        internal int GetGifIndex()
        {
            int count = GetGifCount();

            var random = new Random();
            return random.Next(2, count);
        }

        internal int GetGifCount()
        {
            int count = 1;
            var query = new TableQuery<GifserciseEntity>()
                                    .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "count"));

            foreach (var entity in _table.ExecuteQuery(query))
            {
                Console.WriteLine(entity.PartitionKey + ": " + entity.RowKey);
                count = int.Parse(entity.RowKey);
            }

            return count;
        }

    }
}
