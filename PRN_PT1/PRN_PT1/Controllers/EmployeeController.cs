using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRN_PT1.Models;

namespace PRN_PT1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly Prn232Pt1Context _context;

        public EmployeeController(Prn232Pt1Context context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetAllEmployees()
        {
            return await _context.Employees.ToListAsync();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Employee>>> SearchEmployeeByName(string name)
        {
            var result = await _context.Employees
                .Where(e => e.EmpName.Contains(name))
                .ToListAsync();

            if (!result.Any())
                return NotFound("No employees found");

            return result;
        }


        [HttpPost("/api/orders")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDTO dto)
        {
            var order = new Order
            {
                OrderId = dto.OrderId,
                Status = dto.Status,
                OrderDate = dto.OrderDate,
                EmpId = dto.EmpId
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderId }, order);
        }

        public class OrderCreateDTO
        {
            public int OrderId { get; set; }
            public string? Status { get; set; }
            public DateOnly? OrderDate { get; set; }
            public int? EmpId { get; set; }
        }

        [HttpGet("/api/orders/{id}")]
        public async Task<ActionResult<Order>> GetOrderById(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();
            return order;
        }

        [HttpGet("{empId}/orders")]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrdersByEmployee(int empId)
        {
            var orders = await _context.Orders
                .Where(o => o.EmpId == empId)
                .Select(o => new OrderDTO
                {
                    OrderID = o.OrderId,
                    Status = o.Status,
                    OrderDate = o.OrderDate
                })
                .ToListAsync();

            if (!orders.Any())
                return NotFound("No orders found for this employee");

            return Ok(orders);
        }
        public class OrderDTO
        {
            public int OrderID { get; set; }
            public string? Status { get; set; }
            public DateOnly? OrderDate { get; set; }
        }


        [HttpPut("{id}/address")]
        public async Task<IActionResult> UpdateAddress(int id, [FromBody] string newAddress)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return NotFound();

            employee.Address = newAddress;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
