using BusinessObjects;
using DataAccess.DAOs;
using DataAccess.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class ProductRepository : IProductRepository
    {
        public Task AddProduct(Product product) => ProductDAO.Instance.AddProduct(product);
        public Task DeleteProduct(int id) => ProductDAO.Instance.DeleteProduct(id);
        public Task<Product> GetProduct(int id) => ProductDAO.Instance.GetProduct(id);
        public Task<List<Product>> GetProducts() => ProductDAO.Instance.GetProducts();
        public Task<List<Product>> GetProducts(string query) => ProductDAO.Instance.GetProducts(query);
        public Task UpdateProduct(Product product) => ProductDAO.Instance.UpdateProduct(product);
    }
}
