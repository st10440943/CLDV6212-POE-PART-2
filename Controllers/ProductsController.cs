using Microsoft.AspNetCore.Mvc;
using ABC_Retail.Services;
using System.Linq;
using System.Threading.Tasks;

namespace ABC_Retail.Controllers
{
    public class ProductsController : Controller
    {
        private readonly TableStorageService _tables;

        public ProductsController(TableStorageService tables)
        {
            _tables = tables;
        }

        // ---------------- List all products with search & sort ----------------
        public async Task<IActionResult> Index(string search, string sortOrder)
        {
            var products = await _tables.GetProductsAsync();

            // ----- Filtering -----
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                products = products.Where(p =>
                    p.Name.ToLower().Contains(search) ||
                    p.Size.ToLower().Contains(search)).ToList();
            }

            // ----- Sorting -----
            ViewData["NameSort"] = sortOrder == "name" ? "name_desc" : "name";
            ViewData["PriceSort"] = sortOrder == "price" ? "price_desc" : "price";

            products = sortOrder switch
            {
                "name" => products.OrderBy(p => p.Name).ToList(),
                "name_desc" => products.OrderByDescending(p => p.Name).ToList(),
                "price" => products.OrderBy(p => p.Price).ToList(),
                "price_desc" => products.OrderByDescending(p => p.Price).ToList(),
                _ => products.OrderBy(p => p.Name).ToList()
            };

            ViewData["CurrentFilter"] = search;
            return View(products);
        }

        // ---------------- Seed 6 sample soccer kits ----------------
        public async Task<IActionResult> Seed()
        {
            await _tables.SeedProductsAsync();
            TempData["Message"] = "✅ 6 sample soccer shirts have been added successfully!";
            return RedirectToAction(nameof(Index));
        }

        // ---------------- Delete product (optional admin feature) ----------------
        [HttpPost]
        public async Task<IActionResult> Delete(string partitionKey, string rowKey)
        {
            await _tables.DeleteProductAsync(partitionKey, rowKey);
            TempData["Message"] = "🗑️ Product deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
