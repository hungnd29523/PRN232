using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;

namespace MICHO.Web.Pages.Orders
{
    public class InvoiceModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public InvoiceModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public int OrderID { get; set; }
        public string InvoiceHtml { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            OrderID = id;
            var client = _httpClientFactory.CreateClient("MICHOAPI");
            var res = await client.GetAsync($"orders/{id}/invoice-html");

            if (!res.IsSuccessStatusCode) return NotFound();

            InvoiceHtml = await res.Content.ReadAsStringAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostExportAsync(int id)
        {
            var http = _httpClientFactory.CreateClient("MICHOAPI");
            var response = await http.GetAsync($"orders/{id}/invoice-excel");

            if (!response.IsSuccessStatusCode) return NotFound();

            var file = await response.Content.ReadAsByteArrayAsync();
            var fileName = $"invoice_{id}.xlsx";

            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}
