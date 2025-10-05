using Azure;
using Azure.Data.Tables;

namespace ABC_Retail.Models
{
    public class ProductEntity : ITableEntity
    {
        public string PartitionKey { get; set; } = "PRODUCT";
        public string RowKey { get; set; } = Guid.NewGuid().ToString();
        public string Sku { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public double Price { get; set; }

        public string? ImageUrl { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
