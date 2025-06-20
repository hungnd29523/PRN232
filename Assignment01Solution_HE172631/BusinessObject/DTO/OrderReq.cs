using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class OrderReq
    {
        public int OrderId { get; set; }
        [Required]
        public DateOnly? OrderDate { get; set; }

        [Required]
        public decimal? Freight { get; set; }
        [Required]
        public int MemberId { get; set; }
    }
}
