using Microsoft.AspNetCore.Mvc;
using ABC_Retail.Services;

namespace ABC_Retail.Controllers
{
    public class FilesController : Controller
    {
        private readonly FileShareStorageService _files;

        public FilesController(FileShareStorageService files)
        {
            _files = files;
        }

        // ---------------- List all uploaded contracts ----------------
        public async Task<IActionResult> Index()
        {
            var files = await _files.ListAsync();
            return View(files);
        }

        // ---------------- Upload form ----------------
        public IActionResult Upload() => View();

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                await _files.UploadAsync(file);
                TempData["Message"] = $"{file.FileName} uploaded successfully!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Please select a valid file to upload.");
            return View();
        }
    }
}
