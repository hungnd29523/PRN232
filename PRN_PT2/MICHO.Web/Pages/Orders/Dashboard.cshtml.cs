using System.Text.Json;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MICHO.Web.Pages.Orders
{
    public class DashboardModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public List<(DateTime Date, int TotalOrders, decimal TotalRevenue)> SalesReport { get; set; } = new();
        public List<(int Hour, int Count)> PeakHours { get; set; } = new();
        public List<(string IceName, int Sold)> BestSellers { get; set; } = new();

        public DashboardModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task OnGetAsync()
        {
            var http = _httpClientFactory.CreateClient("MICHOAPI");

            // 1. Get Sales Report
            var salesRes = await http.GetAsync("orders/sales-report");
            if (salesRes.IsSuccessStatusCode)
            {
                var json = await salesRes.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(json);

                SalesReport = doc.RootElement.EnumerateArray()
                    .Select(e => (
                        Date: e.GetProperty("date").GetDateTime(),
                        TotalOrders: e.GetProperty("totalOrders").GetInt32(),
                        TotalRevenue: e.GetProperty("totalRevenue").GetDecimal()
                    ))
                    .ToList();
            }

            // 2. Get Analytics (Peak Hours + Best Sellers)
            var analyticsRes = await http.GetAsync("orders/analytics");
            if (analyticsRes.IsSuccessStatusCode)
            {
                var json = await analyticsRes.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(json);

                PeakHours = doc.RootElement.GetProperty("peakHours")
                    .EnumerateArray()
                    .Select(e => (
                        e.GetProperty("hour").GetInt32(),
                        e.GetProperty("count").GetInt32()
                    ))
                    .ToList();

                BestSellers = doc.RootElement.GetProperty("bestSellers")
                    .EnumerateArray()
                    .Select(e => (
                        e.GetProperty("iceName").GetString()!,
                        e.GetProperty("sold").GetInt32()
                    ))
                    .ToList();
            }
        }
    }
}
