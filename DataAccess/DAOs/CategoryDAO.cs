using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAOs
{
    public class CategoryDAO
    {
        private static CategoryDAO instance = null;
        private static readonly object instanceLock = new object();
        private CategoryDAO() { }

        public static CategoryDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new CategoryDAO();
                    }
                    return instance;
                }
            }
        }

        public async Task<Category> GetCategory(int id)
        {
            FStoreDBContext db = new FStoreDBContext();
            Category category = null;
            category = await db.Categories.FirstOrDefaultAsync(m => m.CategoryId == id);
            return category;
        }

        public async Task<List<Category>> GetCategories(string query)
        {
            FStoreDBContext db = new FStoreDBContext();
            query = query.ToLower();
            List<Category> categorys = await db.Categories.Where(m =>
            m.CategoryName.ToLower().Contains(query)
            || m.CategoryId.ToString().Contains(query)
            ).ToListAsync();
            return categorys;
        }

        public async Task<List<Category>> GetCategories()
        {
            FStoreDBContext db = new FStoreDBContext();
            return await db.Categories.ToListAsync();
        }

        public async Task AddCategory(Category category)
        {
            FStoreDBContext db = new FStoreDBContext();
            db.Categories.Add(category);
            await db.SaveChangesAsync();
        }

        public async Task UpdateCategory(Category category)
        {
            FStoreDBContext db = new FStoreDBContext();
            db.Categories.Update(category);
            await db.SaveChangesAsync();
        }

        public async Task DeleteCategory(int id)
        {
            FStoreDBContext db = new FStoreDBContext();
            Category category = new Category { CategoryId = id };
            db.Categories.Attach(category);
            db.Categories.Remove(category);
            await db.SaveChangesAsync();
        }
    }
}
