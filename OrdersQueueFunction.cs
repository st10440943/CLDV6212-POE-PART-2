using System;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using ABC_Retail.Models;

namespace ABC_Retail_Functions
{
    public class OrdersQueueFunction
    {
        private readonly ILogger<OrdersQueueFunction> _logger;
        private readonly TableClient _ordersTable;

        public OrdersQueueFunction(ILogger<OrdersQueueFunction> logger)
        {
            _logger = logger;

            // Use the connection string from local.settings.json
            string connectionString = Environment.GetEnvironmentVariable("QueueConnection");
            _ordersTable = new TableClient(connectionString, "orders");
            _ordersTable.CreateIfNotExists();
        }

        [Function(nameof(OrdersQueueFunction))]
        public async Task Run([QueueTrigger("orders", Connection = "QueueConnection")] QueueMessage message)
        {
            _logger.LogInformation("Queue trigger received message: {MessageText}", message.MessageText);

            try
            {
                var parts = message.MessageText.Split('|');
                if (parts.Length != 3)
                {
                    _logger.LogError("Invalid message format. Expected 'CustomerId|ProductId|Quantity'.");
                    return;
                }

                string customerId = parts[0];
                string productId = parts[1];
                int quantity = int.Parse(parts[2]);

                // Example: total price would normally be retrieved from ProductEntity
                decimal totalPrice = 0;

                var order = new OrderEntity
                {
                    CustomerId = customerId,
                    ProductId = productId,
                    Quantity = quantity,
                    TotalPrice = totalPrice
                };

                await _ordersTable.UpsertEntityAsync(order);
                _logger.LogInformation("Order saved to table with RowKey: {RowKey}", order.RowKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing queue message");
            }
        }
    }
}
