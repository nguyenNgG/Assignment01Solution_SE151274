using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

#nullable disable

namespace BusinessObjects
{
    public partial class Product
    {
        public Product()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int ProductId { get; set; }

        [Required]
        public int? CategoryId { get; set; }

        [Required]
        [StringLength(40, ErrorMessage = "{0} must not have over {1} characters. ")]
        public string ProductName { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "{0} must not have over {1} characters. ")]
        public string Weight { get; set; }

        public decimal UnitPrice { get; set; }

        public int UnitsInStock { get; set; }

        public virtual Category Category { get; set; }
        [JsonIgnore] public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
