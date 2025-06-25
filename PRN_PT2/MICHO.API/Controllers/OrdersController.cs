using ClosedXML.Excel;
using MICHO.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MICHO.API.Controllers
{
    // MICHO.API - OrdersController.cs (updated for Prn232Pt2Context)

    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly Prn232Pt2Context _context;
        public OrdersController(Prn232Pt2Context context) => _context = context;
        public class PlaceOrderDto
        {
            public CustomerDto Customer { get; set; } = new();
            public List<IceOrderDto> Items { get; set; } = new();
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

        // 1. Allow customer to place orders
        [HttpGet("icecreams")]
        public async Task<IActionResult> GetAllIceCreams()
        {
            var iceList = await _context.IceCreams.ToListAsync();
            return Ok(iceList);
        }

        [HttpPost("place")]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderDto dto)
        {
            if (dto == null || dto.Customer == null || dto.Items == null || dto.Items.Count == 0)
                return BadRequest("Invalid order data.");

            // 1. Tạo Customer
            var customer = new Customer
            {
                Name = dto.Customer.Name,
                Address = dto.Customer.Address,
                Contact = dto.Customer.Contact
            };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // 2. Tạo Order
            // 2. Lấy EmpId đầu tiên nếu có
            var emp = await _context.Employees.FirstOrDefaultAsync();

            // 3. Tạo Order
            var order = new Order
            {
                CustomerId = customer.CustomerId,
                OrderDate = DateTime.Now,
                Status = 1,
                EmpId = emp?.EmpId 
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // 3. Tạo OrderDetail
            var orderDetail = new OrderDetail
            {
                OrderId = order.OrderId,
                Quantity = dto.Items.Sum(i => i.Quantity),
                TotalAmount = 0
            };
            _context.OrderDetails.Add(orderDetail);
            await _context.SaveChangesAsync();

            // 4. Thêm từng kem
            foreach (var item in dto.Items)
            {
                var ice = await _context.IceCreams.FindAsync(item.IceId);
                if (ice == null) continue;

                orderDetail.TotalAmount += ice.Price * item.Quantity;

                _context.OrderDetailIceCreams.Add(new OrderDetailIceCream
                {
                    OrderId = order.OrderId,
                    OrderDetailId = orderDetail.OrderDetailId,
                    IceId = item.IceId
                });
            }


            await _context.SaveChangesAsync();
            return Ok(new { order.OrderId });
        }


        // 2. Export invoice
        [HttpGet("{id}/invoice-excel")]
        public async Task<IActionResult> GetInvoiceExcel(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.OrderDetailIceCreams)
                        .ThenInclude(odi => odi.Ice)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null) return NotFound();

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Invoice");

            ws.Cell(1, 1).Value = $"Invoice for Order #{order.OrderId}";
            ws.Cell(2, 1).Value = $"Customer: {order.Customer?.Name}";
            ws.Cell(3, 1).Value = $"Order Date: {order.OrderDate:dd-MM-yyyy HH:mm}";

            // Header
            ws.Cell(5, 1).Value = "Ice Cream";
            ws.Cell(5, 2).Value = "Flavor";
            ws.Cell(5, 3).Value = "Price";
            ws.Cell(5, 4).Value = "Quantity";

            int row = 6;
            foreach (var detail in order.OrderDetails)
            {
                foreach (var item in detail.OrderDetailIceCreams)
                {
                    ws.Cell(row, 1).Value = item.Ice?.Name;
                    ws.Cell(row, 2).Value = item.Ice?.Flavor;
                    ws.Cell(row, 3).Value = item.Ice?.Price ?? 0;
                    ws.Cell(row, 4).Value = detail.Quantity ?? 1;
                    row++;
                }
            }

            decimal total = order.OrderDetails.Sum(d => d.TotalAmount ?? 0);
            ws.Cell(row + 1, 3).Value = "Total";
            ws.Cell(row + 1, 4).Value = total;

            // Auto fit
            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"invoice_{order.OrderId}.xlsx");
        }
        [HttpGet("{id}/invoice-html")]
        public async Task<IActionResult> GetInvoiceHtml(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                    .ThenInclude(d => d.OrderDetailIceCreams)
                        .ThenInclude(i => i.Ice)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null) return NotFound();

            var html = $"<h1>Invoice for Order #{order.OrderId}</h1>" +
                       $"<p>Customer: {order.Customer?.Name}</p>" +
                       $"<ul>" +
                       string.Join("", order.OrderDetails.Select(d =>
                           $"<li>Qty: {d.OrderDetailIceCreams.Count}, Total: {d.TotalAmount:C}</li>")) +
                       "</ul>";

            return Content(html, "text/html");
        }

        // 3. Generate sales report
        [HttpGet("sales-report")]
        public async Task<IActionResult> SalesReport(DateTime? from = null, DateTime? to = null)
        {
            var query = _context.Orders
                .Include(o => o.OrderDetails)
                .Where(o => o.OrderDate.HasValue);

            if (from.HasValue)
                query = query.Where(o => o.OrderDate.Value.Date >= from.Value.Date);

            if (to.HasValue)
                query = query.Where(o => o.OrderDate.Value.Date <= to.Value.Date);

            var result = await query
                .GroupBy(o => o.OrderDate!.Value.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    TotalOrders = g.Count(),
                    TotalRevenue = g.SelectMany(o => o.OrderDetails)
                                    .Sum(d => d.TotalAmount ?? 0)
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            return Ok(result);
        }


        // 4. Peak hours and best sellers
        [HttpGet("analytics")]
        public async Task<IActionResult> Analytics()
        {
            var peakHours = await _context.Orders
                .Where(o => o.OrderDate.HasValue)
                .GroupBy(o => o.OrderDate!.Value.Hour)
                .Select(g => new
                {
                    Hour = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(g => g.Count)
                .ToListAsync();

            var bestSellers = await _context.OrderDetailIceCreams
                .Include(x => x.Ice)
                .GroupBy(i => new { i.IceId, i.Ice.Name })
                .Select(g => new
                {
                    IceId = g.Key.IceId,
                    IceName = g.Key.Name,
                    Sold = g.Count()
                })
                .OrderByDescending(g => g.Sold)
                .ToListAsync();

            return Ok(new
            {
                PeakHours = peakHours,
                BestSellers = bestSellers
            });
        }

    }

}
