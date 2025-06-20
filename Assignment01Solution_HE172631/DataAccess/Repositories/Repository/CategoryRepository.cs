using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;

namespace DataAccess.Repositories.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        public void SaveCategory(Category category) => CategoryDAO.SaveCategory(category);
        public Category GetCategoryById(int id) => CategoryDAO.FindCategoryById(id);
        public List<Category> GetCategories() => CategoryDAO.GetCategories();
        public void UpdateCategory(Category category) => CategoryDAO.UpdateCategory(category);
        public void DeleteCategory(Category category) => CategoryDAO.DeleteCategory(category);

        public List<Product> GetProducts(int categoryId) => ProductDAO.FindAllProductsByCategoryId(categoryId);

    }
}
