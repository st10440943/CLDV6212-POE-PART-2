public class StorageOptions
{
    public string ConnectionString { get; set; }
    public string BlobContainer { get; set; }
    public string QueueName { get; set; }
    public string CustomersTable { get; set; }
    public string ProductsTable { get; set; }
    public string OrdersTable { get; set; }
    public string FileShare { get; set; }

    
    public string BlobFunctionUrl { get; set; }
}
