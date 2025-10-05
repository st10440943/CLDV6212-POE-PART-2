using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Azure.Data.Tables;

namespace ABC_Retail_Functions
{
    public class TableFunction
    {
        private readonly ILogger<TableFunction> _logger;
        private readonly string _connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        private readonly string _tableName = "orders"; // name of your table

        public TableFunction(ILogger<TableFunction> logger)
        {
            _logger = logger;
        }

        [Function("TableFunction")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "addorder")] HttpRequestData req)
        {
            _logger.LogInformation("TableFunction triggered to add an order.");

            // Parse order data from request body
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var orderId = Guid.NewGuid().ToString();

            // Initialize table client
            var tableClient = new TableClient(_connectionString, _tableName);
            await tableClient.CreateIfNotExistsAsync();

            // Create new entity
            var entity = new TableEntity("OrdersPartition", orderId)
            {
                { "CustomerName", "John Doe" },
                { "Product", "Arsenal Kit" },
                { "Quantity", 1 },
                { "Price", 899.99 },
                { "Date", DateTime.UtcNow }
            };

            await tableClient.AddEntityAsync(entity);

            // Respond with success message
            var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
            await response.WriteStringAsync($"Order {orderId} added to Table '{_tableName}'.");
            return response;
        }
    }
}
