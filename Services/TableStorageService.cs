using ABC_Retail.Models;
using Azure;
using Azure.Data.Tables;

namespace ABC_Retail.Services
{
    public class TableStorageService
    {
        private readonly TableClient _customers;
        private readonly TableClient _products;
        private readonly TableClient _orders;

        public TableStorageService(StorageOptions opt)
        {
            _customers = new TableClient(opt.ConnectionString, opt.CustomersTable);
            _products = new TableClient(opt.ConnectionString, opt.ProductsTable);
            _orders = new TableClient(opt.ConnectionString, opt.OrdersTable);

            _customers.CreateIfNotExists();
            _products.CreateIfNotExists();
            _orders.CreateIfNotExists();
        }

        // ---------------- Customers ----------------
        public async Task<List<CustomerEntity>> GetCustomersAsync()
        {
            var list = new List<CustomerEntity>();
            await foreach (var c in _customers.QueryAsync<CustomerEntity>(c => c.PartitionKey == "CUSTOMER"))
                list.Add(c);
            return list;
        }

        public async Task<CustomerEntity?> GetCustomerByIdAsync(string id)
        {
            try
            {
                return (await _customers.GetEntityAsync<CustomerEntity>("CUSTOMER", id)).Value;
            }
            catch (RequestFailedException)
            {
                return null;
            }
        }

        public async Task UpsertCustomerAsync(CustomerEntity c)
            => await _customers.UpsertEntityAsync(c);

        public async Task SeedCustomersAsync()
        {
            for (int i = 1; i <= 5; i++)
            {
                await UpsertCustomerAsync(new CustomerEntity
                {
                    FirstName = $"Customer{i}",
                    LastName = "Test",
                    Email = $"customer{i}@abc.com"
                });
            }
        }

        // ---------------- Products ----------------
        public async Task<List<ProductEntity>> GetProductsAsync()
        {
            var list = new List<ProductEntity>();
            await foreach (var p in _products.QueryAsync<ProductEntity>(p => p.PartitionKey == "PRODUCT"))
                list.Add(p);
            return list;
        }

        public async Task<ProductEntity?> GetProductByIdAsync(string id)
        {
            try
            {
                return (await _products.GetEntityAsync<ProductEntity>("PRODUCT", id)).Value;
            }
            catch (RequestFailedException)
            {
                return null;
            }
        }

        public async Task UpsertProductAsync(ProductEntity p)
            => await _products.UpsertEntityAsync(p);

        public async Task SeedProductsAsync()
        {
            var products = new[]
            {
        new { Name = "Spurs Kit", Price = 899.99, ImageUrl = "https://st10440943.blob.core.windows.net/product-images/SpursKit.png" },
        new { Name = "Arsenal Kit", Price = 899.99, ImageUrl = "https://st10440943.blob.core.windows.net/product-images/ArsenalKit.png" },
        new { Name = "Liverpool Kit", Price = 899.99, ImageUrl = "https://st10440943.blob.core.windows.net/product-images/LiverpoolKit.png" },
        new { Name = "Chelsea Kit", Price = 899.99, ImageUrl = "https://st10440943.blob.core.windows.net/product-images/ChelseaKit.png" },
        new { Name = "Man U Kit", Price = 899.99, ImageUrl = "https://st10440943.blob.core.windows.net/product-images/ManUKit.png" },
        new { Name = "Man City Kit", Price = 899.99, ImageUrl = "https://st10440943.blob.core.windows.net/product-images/ManCityKit.png" }
    };

            int i = 1;
            foreach (var p in products)
            {
                await UpsertProductAsync(new ProductEntity
                {
                    Sku = $"SKU{i}",
                    Name = p.Name,
                    Size = "M",
                    Price = p.Price,
                    ImageUrl = p.ImageUrl
                });
                i++;
            }
        }

        public async Task DeleteProductAsync(string partitionKey, string rowKey)
        {
            await _products.DeleteEntityAsync(partitionKey, rowKey);
        }



        // ---------------- Orders ----------------
        public async Task<List<OrderEntity>> GetOrdersAsync()
        {
            var list = new List<OrderEntity>();
            await foreach (var o in _orders.QueryAsync<OrderEntity>(o => o.PartitionKey == "ORDER"))
                list.Add(o);
            return list;
        }

        public async Task UpsertOrderAsync(OrderEntity o)
            => await _orders.UpsertEntityAsync(o); // ONLY called by QueueTrigger function
    }
}
