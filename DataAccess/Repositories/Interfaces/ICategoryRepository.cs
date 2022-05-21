using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        public Task<List<Category>> GetCategories();
        public Task<List<Category>> GetCategories(string query);
        public Task<Category> GetCategory(int id);
        public Task AddCategory(Category category);
        public Task UpdateCategory(Category category);
        public Task DeleteCategory(int id);
    }
}
