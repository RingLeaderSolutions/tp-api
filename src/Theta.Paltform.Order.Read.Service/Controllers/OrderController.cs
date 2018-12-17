using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Theta.Paltform.Order.Read.Service.Framework;

namespace Theta.Paltform.Order.Read.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private IMemoryCache _cache;

        public OrderController(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<Domain.Order> Get(Guid orderId)
        {
            List<object> events = new List<object>();

            if (_cache.TryGetValue<List<object>>(orderId, out events))
            {
                var order = (IAggregateRoot)Activator.CreateInstance(typeof(Domain.Order), true);
                events.ForEach(order.Apply);
                order.ClearEvents();

                return Ok(order);
            }

            return NotFound();
        }
    }
}
