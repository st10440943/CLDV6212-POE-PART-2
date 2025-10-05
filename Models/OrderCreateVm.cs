using Microsoft.AspNetCore.Mvc.Rendering;

namespace ABCRetail.Web.Models
{
    public class OrderCreateVm
    {
        public string CustomerId { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }

        // For dropdowns
        public List<SelectListItem> Customers { get; set; } = new();
        public List<SelectListItem> Products { get; set; } = new();
    }
}
