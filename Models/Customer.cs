using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;

namespace ABC_Retail.Models { 
public class CustomerEntity : ITableEntity
{
    public string PartitionKey { get; set; } = "CUSTOMER";
    public string RowKey { get; set; } = Guid.NewGuid().ToString();
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }


    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
}
}
