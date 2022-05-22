using System.Collections.Generic;

namespace eStoreAPI.Models
{
    public class Cart
    {
        public Cart()
        {
            CartDetails = new List<CartDetail>();
        }

        public List<CartDetail> CartDetails { get; set; }
    }
}