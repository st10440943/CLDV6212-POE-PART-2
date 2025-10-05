using Microsoft.AspNetCore.Mvc;
using ABC_Retail.Models;
using ABC_Retail.Services;

namespace ABC_Retail.Controllers
{
    public class CustomersController : Controller
    {
        private readonly TableStorageService _tables;

        public CustomersController(TableStorageService tables)
        {
            _tables = tables;
        }

        // ---------------- List all customers ----------------
        public async Task<IActionResult> Index()
        {
            var customers = await _tables.GetCustomersAsync();
            return View(customers);
        }

        // ---------------- Seed 5 sample customers ----------------
        public async Task<IActionResult> Seed()
        {
            await _tables.SeedCustomersAsync();
            TempData["Message"] = "5 sample customers have been added successfully!";
            return RedirectToAction(nameof(Index));
        }

        // ---------------- Show create form ----------------
        public IActionResult Create() => View();

        // ---------------- Handle form post ----------------
        [HttpPost]
        public async Task<IActionResult> Create(CustomerEntity customer)
        {
            if (ModelState.IsValid)
            {
                customer.PartitionKey = "CUSTOMER";
                customer.RowKey = Guid.NewGuid().ToString();

                await _tables.UpsertCustomerAsync(customer);
                TempData["Message"] = $"Customer {customer.FirstName} added successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(customer);
        }
    }
}
