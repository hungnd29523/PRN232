﻿using DataAccess.Repositories.Repository;
using DataAccess.Repositories;
using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;

namespace eStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private ICategoryRepository repository = new CategoryRepository();

        [HttpGet]
        public ActionResult<IEnumerable<Category>> GetCategories() => repository.GetCategories();

        [HttpGet("id")]
        public ActionResult<Category> GetCategoryById(int id) => repository.GetCategoryById(id);

        [HttpPost]
        public IActionResult PostCategory(Category category)
        {
            repository.SaveCategory(category);
            return NoContent();
        }

        [HttpDelete("id")]
        public IActionResult DeleteCategory(int id)
        {
            var c = repository.GetCategoryById(id);
            if (c == null)
            {
                return NotFound();
            }
            repository.DeleteCategory(c);
            return NoContent();
        }

        [HttpPut("id")]
        public IActionResult PutCategory(int id, Category category)
        {
            var cTmp = repository.GetCategoryById(id);
            if (cTmp == null)
            {
                return NotFound();
            }

            cTmp.CategoryName = category.CategoryName;

            repository.UpdateCategory(cTmp);
            return NoContent();
        }
    }
}
