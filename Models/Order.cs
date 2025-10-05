using Azure;
using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations;

namespace ABC_Retail.Models
{
    public class OrderEntity : ITableEntity
    {
        public string PartitionKey { get; set; } = "ORDER";
        public string RowKey { get; set; } = Guid.NewGuid().ToString();
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        [Required]
        public string CustomerId { get; set; }   // RowKey of CustomerEntity

        [Required]
        public string ProductId { get; set; }    // RowKey of ProductEntity

        [Required]
        public int Quantity { get; set; }

        public decimal TotalPrice { get; set; }

        // Additional properties for display
        public string CustomerName { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
    }
}
