﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Theta.Platform.Domain;

namespace Theta.Platform.Order.Management.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IAggregateReader<Domain.Order> _aggregateReader;

        public OrderController(IAggregateReader<Domain.Order> aggregateReader)
        {
            this._aggregateReader = aggregateReader;
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> Get(Guid orderId)
        {
            var order = _aggregateReader.GetById(orderId);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }
    }
}
