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
    public class OrderDetailController : ControllerBase
    {
        IOrderDetailRepository repository;
        public OrderDetailController(IOrderDetailRepository _repository)
        {
            repository = _repository;
        }

        // GET: by id
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderDetail))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderDetail>> GetOrderDetail(int orderId, int productId)
        {
            OrderDetail orderDetail = await repository.GetOrderDetail(orderId, productId);
            if (orderDetail == null)
            {
                return NotFound();
            }
            return Ok(orderDetail);
        }

        // GET: list
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<OrderDetail>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<OrderDetail>>> GetOrderDetails(int orderId)
        {
            List<OrderDetail> orderDetails = null;
            orderDetails = await repository.GetOrderDetails(orderId);

            if (orderDetails == null)
            {
                return NotFound();
            }
            return Ok(orderDetails);
        }

        //// ADD
        //[HttpPost]
        //[Consumes(MediaTypeNames.Application.Json)]
        //[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(OrderDetail))]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<ActionResult<OrderDetail>> AddOrderDetail(OrderDetail orderDetail)
        //{
        //    try
        //    {
        //        await repository.AddOrderDetail(orderDetail);
        //    }
        //    catch (DbUpdateException)
        //    {
        //        if (await repository.GetOrderDetail(orderDetail.OrderDetailId) != null)
        //        {
        //            return BadRequest();
        //        }
        //    }
        //    return CreatedAtAction("GetOrderDetail", new { id = orderDetail.OrderDetailId }, orderDetail);
        //}

        //// UPDATE
        //[HttpPut("{id}")]
        //[Consumes(MediaTypeNames.Application.Json)]
        //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderDetail))]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<ActionResult<OrderDetail>> UpdateOrderDetail(int id, OrderDetail orderDetail)
        //{
        //    if (id != orderDetail.OrderDetailId)
        //    {
        //        return BadRequest();
        //    }

        //    try
        //    {
        //        await repository.UpdateOrderDetail(orderDetail);
        //    }

        //    catch (DbUpdateException)
        //    {
        //        if (await repository.GetOrderDetail(orderDetail.OrderDetailId) == null)
        //        {
        //            return NotFound();
        //        }
        //    }
        //    return Ok(orderDetail);
        //}

        //// DELETE
        //[HttpDelete("{id}")]
        //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderDetail))]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<ActionResult<OrderDetail>> DeleteOrderDetail(int id)
        //{
        //    try
        //    {
        //        await repository.DeleteOrderDetail(id);
        //    }

        //    catch (DbUpdateException)
        //    {
        //        if (await repository.GetOrderDetail(id) == null)
        //        {
        //            return NotFound();
        //        }
        //    }
        //    return NoContent();
        //}

    }
}
