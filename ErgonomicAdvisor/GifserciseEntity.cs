using Microsoft.WindowsAzure.Storage.Table;

namespace ErgonomicAdvisor
{
    public class GifserciseEntity : TableEntity
    {
        public GifserciseEntity(string partition, string row)
        {
            this.PartitionKey = partition;
            this.RowKey = row;
        }
        public GifserciseEntity() { }

        public string text { get; set; }
        public string image_url { get; set; }
    }
}
