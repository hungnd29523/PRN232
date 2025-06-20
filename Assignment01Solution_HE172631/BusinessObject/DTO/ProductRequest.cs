using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class ProductRequest
    {
        public int ProductId { get; set; }
        [Required]
        public string ProductName { get; set; }

        [Required]
        [Range(1, Int32.MaxValue)]
        public decimal UnitPrice { get; set; }
        [Required]

        public string Weight { get; set; }

        [Required]
        [Range(1, Int32.MaxValue)]
        public int UnitsInStock { get; set; }

        [Required]
        public int CategoryId { get; set; }

    }
}
