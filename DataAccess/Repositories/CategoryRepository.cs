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
    public class CategoryRepository : ICategoryRepository
    {
        public Task AddCategory(Category category) => CategoryDAO.Instance.AddCategory(category);
        public Task DeleteCategory(int id) => CategoryDAO.Instance.DeleteCategory(id);
        public Task<List<Category>> GetCategories() => CategoryDAO.Instance.GetCategories();
        public Task<List<Category>> GetCategories(string query) => CategoryDAO.Instance.GetCategories(query);
        public Task<Category> GetCategory(int id) => CategoryDAO.Instance.GetCategory(id);
        public Task UpdateCategory(Category category) => CategoryDAO.Instance.UpdateCategory(category);
    }
}
