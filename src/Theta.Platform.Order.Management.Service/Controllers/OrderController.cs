using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Theta.Platform.Order.Management.Service.Data;

namespace Theta.Platform.Order.Management.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            this._orderRepository = orderRepository;
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> Get(Guid orderId)
        {
            var orders = await _orderRepository.GetOrder(orderId);

            if (orders == null || orders.Any() == false)
            {
                return NotFound();
            }

            return Ok(orders);
        }

        [HttpGet("entity/{entityId}")]
        public async Task<IActionResult> GetByEntityId(Guid entityId)
        {
            var orders = await _orderRepository.GetOrdersForEntity(entityId);

            if (orders == null || orders.Any() == false)
            {
                return NotFound();
            }

            return Ok(orders);
        }

        [HttpGet("owner/{ownerId}")]
        public async Task<IActionResult> GetByOwnerId(Guid ownerId)
        {
            var orders = await _orderRepository.GetOrdersForOwner(ownerId);

            if (orders == null || orders.Any() == false)
            {
                return NotFound();
            }

            return Ok(orders);
        }
    }
}
