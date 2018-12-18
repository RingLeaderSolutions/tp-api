using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
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

        [HttpGet]
        public ActionResult<Domain.Order> Get(Guid orderId)
        {
            var order = _orderReader.GetById(orderId);

            if (order == null)
                return NotFound();

            return Ok(order);
        }
    }
}
