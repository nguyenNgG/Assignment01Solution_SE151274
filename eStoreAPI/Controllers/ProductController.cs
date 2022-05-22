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
    [Route("api/[controller]s")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        IProductRepository repository;
        public ProductController(IProductRepository _repository)
        {
            repository = _repository;
        }

        // GET: by id
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Product))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            Product product = await repository.GetProduct(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        // GET: list
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Product>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<Product>>> GetProducts(string query)
        {
            List<Product> products = null;
            if (query != null && query.Length > 0)
            {
                products = await repository.GetProducts(query);
            }
            else
            {
                products = await repository.GetProducts();
            }

            if (products == null)
            {
                return NotFound();
            }
            return Ok(products);
        }

        // ADD
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Product))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<Product>> AddProduct(Product product)
        {
            try
            {
                await repository.AddProduct(product);
                return CreatedAtAction("GetProduct", new { id = product.ProductId }, product);
            }
            catch (DbUpdateException)
            {
                if (await repository.GetProduct(product.ProductId) != null)
                {
                    return Conflict();
                }
                return BadRequest();
            }
        }

        // UPDATE
        [HttpPut("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Product))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Product>> UpdateProduct(int id, Product product)
        {
            if (id != product.ProductId)
            {
                return BadRequest();
            }

            try
            {
                await repository.UpdateProduct(product);
                return Ok(product);
            }

            catch (DbUpdateException)
            {
                if (await repository.GetProduct(product.ProductId) == null)
                {
                    return NotFound();
                }
                return BadRequest();
            }
        }

        // DELETE
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            try
            {
                await repository.DeleteProduct(id);
                return NoContent();
            }

            catch (DbUpdateException)
            {
                if (await repository.GetProduct(id) == null)
                {
                    return NotFound();
                }
                return BadRequest();
            }
        }

    }
}
