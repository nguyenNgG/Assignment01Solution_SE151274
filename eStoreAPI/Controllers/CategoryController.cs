using BusinessObjects;
using DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;

namespace eStoreAPI.Controllers
{
    [Route("api/Categories")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        ICategoryRepository repository;
        public CategoryController(ICategoryRepository _repository)
        {
            repository = _repository;
        }

        // GET: by id
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Category))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            Category category = await repository.GetCategory(id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        // GET: list
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Category>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<Category>>> GetCategories(string query)
        {
            List<Category> categorys = null;
            if (query != null && query.Length > 0)
            {
                categorys = await repository.GetCategories(query);
            }
            else
            {
                categorys = await repository.GetCategories();
            }

            if (categorys == null)
            {
                return NotFound();
            }
            return Ok(categorys);
        }

        // ADD
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Category))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Category>> AddCategory(Category category)
        {
            try
            {
                await repository.AddCategory(category);
            }
            catch (DbUpdateException)
            {
                if (await repository.GetCategory(category.CategoryId) != null)
                {
                    return BadRequest();
                }
            }
            return CreatedAtAction("GetCategory", new { id = category.CategoryId }, category);
        }

        // UPDATE
        [HttpPut("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Category))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Category>> UpdateCategory(int id, Category category)
        {
            if (id != category.CategoryId)
            {
                return BadRequest();
            }

            try
            {
                await repository.UpdateCategory(category);
            }

            catch (DbUpdateException)
            {
                if (await repository.GetCategory(category.CategoryId) == null)
                {
                    return NotFound();
                }
            }
            return Ok(category);
        }

        // DELETE
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Category))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Category>> DeleteCategory(int id)
        {
            try
            {
                await repository.DeleteCategory(id);
            }

            catch (DbUpdateException)
            {
                if (await repository.GetCategory(id) == null)
                {
                    return NotFound();
                }
            }
            return NoContent();
        }

    }
}
