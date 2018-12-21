using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Theta.Platform.Domain;

namespace Theta.Platform.Order.Management.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IAggregateReader<Domain.Aggregate.Order> _aggregateReader;

        public OrderController(IAggregateReader<Domain.Aggregate.Order> aggregateReader)
        {
            this._aggregateReader = aggregateReader;
        }

        [HttpGet("{orderId}")]
        public IActionResult Get(Guid orderId)
        {
            var order = _aggregateReader.GetById(orderId);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        [HttpGet]
        public IActionResult Get()
        {
	        var orders = _aggregateReader.Get();

	        return Ok(orders);
        }
	}
}
