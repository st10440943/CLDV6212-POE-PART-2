using Microsoft.AspNetCore.Mvc;
using ABC_Retail.Services;

namespace ABC_Retail.Controllers
{
    public class QueueController : Controller
    {
        private readonly QueueStorageService _queue;

        public QueueController(QueueStorageService queue)
        {
            _queue = queue;
        }

        // ---------------- View top 10 messages ----------------
        public async Task<IActionResult> Index()
        {
            var messages = await _queue.PeekMessagesAsync(10);
            return View(messages);
        }
    }
}
