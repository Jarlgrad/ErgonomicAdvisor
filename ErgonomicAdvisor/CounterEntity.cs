using Microsoft.WindowsAzure.Storage.Table;

namespace ErgonomicAdvisor
{
    public class CounterEntity : TableEntity
    {
        public CounterEntity(string count)
        {
            this.PartitionKey = "count";
            this.RowKey = count;
            this.ETag = "*";
        }
        public CounterEntity() { }

    }
}
