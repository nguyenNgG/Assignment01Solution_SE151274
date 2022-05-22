using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

#nullable disable

namespace BusinessObjects
{
    public partial class Member
    {
        public Member()
        {
            Orders = new HashSet<Order>();
        }

        public int MemberId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} must not have over {1} characters. ")]
        public string Email { get; set; }

        [Required]
        [StringLength(40, ErrorMessage = "{0} must not have over {1} characters. ")]
        public string CompanyName { get; set; }

        [Required]
        [StringLength(15, ErrorMessage = "{0} must not have over {1} characters. ")]
        public string City { get; set; }

        [Required]
        [StringLength(15, ErrorMessage = "{0} must not have over {1} characters. ")]
        public string Country { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "{0} must not have over {1} characters. ")]
        public string Password { get; set; }

        [JsonIgnore]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
