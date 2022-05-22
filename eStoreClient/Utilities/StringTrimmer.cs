using BusinessObjects;

namespace eStoreClient.Utilities
{
    public static class StringTrimmer
    {
        public static Member TrimMember(Member member)
        {
            if (member != null)
            {
                member.CompanyName = member.CompanyName.Trim();
                member.City = member.City.Trim();
                member.Country = member.Country.Trim();
                member.Email = member.Email.Trim();
            }
            return member;
        }

        public static Product TrimProduct(Product product)
        {
            if (product != null)
            {
                product.Weight = product.Weight.Trim();
                product.ProductName = product.ProductName.Trim();
            }
            return product;
        }
    }
}
