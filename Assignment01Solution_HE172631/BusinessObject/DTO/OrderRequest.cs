using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class OrderRequest
    {
        public decimal Freight { get; set; }
        public int MemberId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
