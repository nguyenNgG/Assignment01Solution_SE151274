using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Interfaces
{
    public interface IProductRepository
    {
        public Task<List<Product>> GetProducts();
        public Task<List<Product>> GetProducts(string query);
        public Task<Product> GetProduct(int id);
        public Task AddProduct(Product product);
        public Task UpdateProduct(Product product);
        public Task DeleteProduct(int id);
    }
}
