using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace ErgonomicAdvisor
{
    public class GifserciseEntity : TableEntity
    {
        public GifserciseEntity(string partition, string row, string url, string text)
        {
            this.PartitionKey = partition;
            this.RowKey = row;
            this.text = text;
            this.image_url = url;
            this.Timestamp = DateTime.Now;
        }
        public GifserciseEntity() { }

        public string text { get; set; }
        public string image_url { get; set; }
    }
}
