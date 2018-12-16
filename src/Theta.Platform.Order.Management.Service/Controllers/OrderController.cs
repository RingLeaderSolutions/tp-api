using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Theta.Platform.Order.Management.Service.Framework;

namespace Theta.Platform.Order.Management.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IAggregateRepository _orderRepository;

        public OrderController(IAggregateRepository orderRepository)
        {
            this._orderRepository = orderRepository;
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> Get(Guid orderId)
        {
            var order = await _orderRepository.GetAsync<Domain.Order>(orderId);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }
    }
}
