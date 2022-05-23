using System;
using System.ComponentModel.DataAnnotations;

namespace eStoreClient.Models
{
    public class EditOrderForm
    {
        [DataType(DataType.DateTime)]
        [Display(Name = "Shipped Date")]
        public DateTime? ShippedDate { get; set; }

        [Required(ErrorMessage = "{0} is required. ")]
        public decimal? Freight { get; set; }
    }
}
