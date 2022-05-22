using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAOs
{
    public class ProductDAO
    {
        private static ProductDAO instance = null;
        private static readonly object instanceLock = new object();
        private ProductDAO() { }

        public static ProductDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new ProductDAO();
                    }
                    return instance;
                }
            }
        }

        public async Task<Product> GetProduct(int id)
        {
            FStoreDBContext db = new FStoreDBContext();
            Product product = null;
            product = await db.Products.Include(m => m.Category).FirstOrDefaultAsync(m => m.ProductId == id);
            return product;
        }

        public async Task<List<Product>> GetProducts(string query)
        {
            FStoreDBContext db = new FStoreDBContext();
            query = query.ToLower();
            List<Product> products = await db.Products.Include(m => m.Category).Where(m =>
            m.ProductId.ToString().Contains(query)
            || m.ProductName.ToLower().Contains(query)
            || m.UnitPrice.ToString().Contains(query)
            || m.Weight.ToLower().Contains(query)
            || m.UnitsInStock.ToString().Contains(query)
            ).ToListAsync();
            return products;
        }

        public async Task<List<Product>> GetProducts()
        {
            FStoreDBContext db = new FStoreDBContext();
            return await db.Products.Include(m => m.Category).ToListAsync();
        }

        public async Task AddProduct(Product product)
        {
            FStoreDBContext db = new FStoreDBContext();
            db.Products.Add(product);
            await db.SaveChangesAsync();
        }

        public async Task UpdateProduct(Product product)
        {
            FStoreDBContext db = new FStoreDBContext();
            db.Products.Update(product);
            await db.SaveChangesAsync();
        }

        public async Task DeleteProduct(int id)
        {
            FStoreDBContext db = new FStoreDBContext();
            Product product = new Product { ProductId = id };
            db.Products.Attach(product);
            db.Products.Remove(product);
            await db.SaveChangesAsync();
        }
    }
}
