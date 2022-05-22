namespace eStoreAPI.Models
{
    public class CartDetail
    {
        public ProductItem ProductItem { get; set; }
        public short Quantity { get; set; } = 0;
    }
}
