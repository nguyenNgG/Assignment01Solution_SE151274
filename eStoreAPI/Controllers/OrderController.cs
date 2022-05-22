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
    public class OrderController : ControllerBase
    {
        IOrderRepository repository;
        public OrderController(IOrderRepository _repository)
        {
            repository = _repository;
        }

        // GET: by id
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Order))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            Order order = await repository.GetOrder(id);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }

        // GET: list
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Order>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<Order>>> GetOrders(int? memberId)
        {
            List<Order> orders = null;
            if (memberId != null)
            {
                orders = await repository.GetOrders((int)memberId);
            }
            else
            {
                orders = await repository.GetOrders();
            }

            if (orders == null)
            {
                return NotFound();
            }
            return Ok(orders);
        }

        // ADD
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Order))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Order>> AddOrder(Order order)
        {
            try
            {
                await repository.AddOrder(order);
            }
            catch (DbUpdateException)
            {
                if (await repository.GetOrder(order.OrderId) != null)
                {
                    return BadRequest();
                }
            }
            return CreatedAtAction("GetOrder", new { id = order.OrderId }, order);
        }

        // UPDATE
        [HttpPut("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Order))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Order>> UpdateOrder(int id, Order order)
        {
            if (id != order.OrderId)
            {
                return BadRequest();
            }

            try
            {
                await repository.UpdateOrder(order);
            }

            catch (DbUpdateException)
            {
                if (await repository.GetOrder(order.OrderId) == null)
                {
                    return NotFound();
                }
            }
            return Ok(order);
        }

        // DELETE
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Order))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Order>> DeleteOrder(int id)
        {
            try
            {
                await repository.DeleteOrder(id);
            }

            catch (DbUpdateException)
            {
                if (await repository.GetOrder(id) == null)
                {
                    return NotFound();
                }
            }
            return NoContent();
        }

    }
}
