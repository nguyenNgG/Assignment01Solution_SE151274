﻿using System;
using System.Collections.Generic;
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
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Password { get; set; }

        [JsonIgnore]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
