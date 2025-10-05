using ABC_Retail.Models;
using ABC_Retail.Services;
using ABCRetail.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ABC_Retail.Controllers
{
    public class OrdersController : Controller
    {
        private readonly TableStorageService _tables;
        private readonly QueueStorageService _queue;

        public OrdersController(TableStorageService tables, QueueStorageService queue)
        {
            _tables = tables;
            _queue = queue;
        }

        // ---------------- List all orders ----------------
        public async Task<IActionResult> Index(string searchCustomer = "", string searchProduct = "")
        {
            var orders = await _tables.GetOrdersAsync();

            if (!string.IsNullOrEmpty(searchCustomer))
                orders = orders.Where(o => o.CustomerName != null &&
                                           o.CustomerName.Contains(searchCustomer, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrEmpty(searchProduct))
                orders = orders.Where(o => o.ProductName != null &&
                                           o.ProductName.Contains(searchProduct, StringComparison.OrdinalIgnoreCase)).ToList();

            ViewBag.SearchCustomer = searchCustomer;
            ViewBag.SearchProduct = searchProduct;

            return View(orders.OrderByDescending(o => o.CreatedDate));
        }

        // ---------------- Show create form ----------------
        public async Task<IActionResult> Create()
        {
            var customers = await _tables.GetCustomersAsync();
            var products = await _tables.GetProductsAsync();

            var vm = new OrderCreateVm
            {
                Customers = customers.Select(c => new SelectListItem
                {
                    Value = c.RowKey,
                    Text = $"{c.FirstName} {c.LastName}"
                }).ToList(),

                Products = products.Select(p => new SelectListItem
                {
                    Value = p.RowKey,
                    Text = $"{p.Name} (R{p.Price})"
                }).ToList()
            };

            return View(vm);
        }

        // ---------------- Save new order ----------------
        [HttpPost]
        public async Task<IActionResult> Create(string customerId, string productId, int quantity)
        {
            var customer = await _tables.GetCustomerByIdAsync(customerId);
            var product = await _tables.GetProductByIdAsync(productId);

            if (customer == null || product == null)
                return BadRequest("Invalid customer or product");

            // Queue message format: CustomerRowKey|ProductRowKey|Quantity
            var message = $"{customer.RowKey}|{product.RowKey}|{quantity}";
            await _queue.SendMessageAsync(message);

            TempData["Message"] = $"Order for {quantity} x {product.Name} has been queued!";
            return RedirectToAction(nameof(Index));
        }

        // ---------------- Update Order Status ----------------
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(string partitionKey, string rowKey, string status)
        {
            var order = (await _tables.GetOrdersAsync())
                        .FirstOrDefault(o => o.PartitionKey == partitionKey && o.RowKey == rowKey);

            if (order != null)
            {
                order.Status = status;
                await _tables.UpsertOrderAsync(order);
                TempData["Message"] = $"Order {rowKey} status updated to {status}.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
