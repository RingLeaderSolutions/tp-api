using System;
using Microsoft.AspNetCore.Mvc;
using Theta.Paltform.Order.Read.Service.Data;

namespace Theta.Paltform.Order.Read.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private IOrderReader _orderReader;

        public OrderController(IOrderReader orderReader)
        {
            _orderReader = orderReader;
        }

		[HttpGet("{orderId}")]
		public ActionResult<Domain.Order> Get(Guid orderId)
        {
            var order = _orderReader.GetById(orderId);

            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [HttpGet]
        public IActionResult Get()
        {
	        var orders = _orderReader.Get();

	        return Ok(orders);
        }
	}
}
