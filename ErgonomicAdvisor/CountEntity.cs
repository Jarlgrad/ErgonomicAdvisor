using Microsoft.WindowsAzure.Storage.Table;

namespace ErgonomicAdvisor
{
    public class CountEntity : TableEntity
    {
        public CountEntity(string count)
        {
            this.PartitionKey = "count";
            this.RowKey = count;
        }
        public CountEntity() { }

    }
}
