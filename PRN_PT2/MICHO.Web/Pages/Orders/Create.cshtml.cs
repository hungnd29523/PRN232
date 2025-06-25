using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;

namespace MICHO.Web.Pages.Orders;

public class CreateModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CreateModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public List<IceCreamDto> IceCreams { get; set; } = new();

    [BindProperty]
    public CustomerDto Customer { get; set; } = new();

    [BindProperty]
    public List<IceOrderDto> SelectedItems { get; set; } = new();

    public async Task OnGetAsync()
    {
        var client = _httpClientFactory.CreateClient("MICHOAPI");
        var res = await client.GetAsync("orders/icecreams");
        Console.WriteLine("IceCreams fetched: " + IceCreams.Count);

        if (res.IsSuccessStatusCode)
        {
            var json = await res.Content.ReadAsStringAsync();
            IceCreams = JsonSerializer.Deserialize<List<IceCreamDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;

            SelectedItems = IceCreams.Select(i => new IceOrderDto { IceId = i.IceId, Quantity = 0 }).ToList();
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Failed to load ice cream list.");
        }
    }


    public async Task<IActionResult> OnPostAsync()
    {
        var client = _httpClientFactory.CreateClient("MICHOAPI");

        var payload = new
        {
            customer = Customer,
            items = SelectedItems.Where(i => i.Quantity > 0).ToList()
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var res = await client.PostAsync("orders/place", content);
        if (!res.IsSuccessStatusCode)
        {
            ModelState.AddModelError(string.Empty, "Failed to place order.");
            return Page();
        }

        var responseData = JsonSerializer.Deserialize<Dictionary<string, int>>(await res.Content.ReadAsStringAsync());
        int orderId = responseData["orderId"];
        return RedirectToPage("Invoice", new { id = orderId });
    }

    public class IceCreamDto
    {
        public int IceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Flavor { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    public class CustomerDto
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Contact { get; set; } = string.Empty;
    }

    public class IceOrderDto
    {
        public int IceId { get; set; }
        public int Quantity { get; set; }
    }
}
