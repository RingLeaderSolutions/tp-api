using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Theta.Platform.Messaging.Commands;
using Theta.Platform.UI.Orders.API.Configuration;
using Theta.Platform.UI.Orders.API.Domain;
using Theta.Platform.UI.Orders.API.Domain.Commands;
using Theta.Platform.UI.Orders.API.Domain.Requests;
using Theta.Platform.UI.Orders.API.Services;

namespace Theta.Platform.UI.Orders.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public sealed class OrderController : ControllerBase
	{
		private readonly OrderApiConfiguration _configuration;
		private readonly IOrderReader _orderReader;
		private readonly ICommandQueueClient _queueClient;

		public OrderController(
			OrderApiConfiguration configuration,
			IOrderReader orderReader,
			ICommandQueueClient queueClient)
		{
			_configuration = configuration;
			_orderReader = orderReader;
			_queueClient = queueClient;
		}

		/// <summary>
		/// Retrieve an order by its order id
		/// </summary>
		/// <param name="orderId">The id of the order</param>
		/// <returns>An object representing the order</returns>
		[HttpGet("{orderId}")]
		public ActionResult<Order> Get(Guid orderId)
		{
			var order = _orderReader.GetById(orderId);

			if (order == null)
				return NotFound();

			return Ok(order);
		}

		/// <summary>
		/// Request submission of an order
		/// </summary>
		/// <param name="request">An object representing the order parameters</param>
		/// <returns>An object containing the submitted order id</returns>
		[HttpPost("submit")]
		public async Task<ActionResult> Submit(SubmitOrderRequest request)
		{
			// TODO: Validate SubmitOrderRequest
			// TODO: Retrieve desk, ownerId from user context
			// TODO: Retrieve currency from Instrument
			// TODO: Ensure user has Sales role
			var orderId = Guid.NewGuid();
			var command = new CreateOrderCommand(
				Guid.NewGuid(), 
				orderId, 
				null, 
				request.Instrument, 
				Guid.NewGuid(),
				request.Quantity,
				request.Side,
				request.Type.ToString(),
				request.LimitPrice,
				"GBP",
				request.Markup.Unit.ToString(),
				request.Markup.Value,
				request.Expiry.GoodTillDate,
				request.Expiry.Type.ToString());

			await _queueClient.Send(_configuration.CommandQueueName, command);

			return Ok(new SubmitOrderResponse(orderId));
		}

		/// <summary>
		/// Request pickup of an order
		/// </summary>
		/// <param name="orderId">The id of the order to pick up</param>
		/// <returns>200 OK if the request was successfully processed.</returns>
		[HttpPost("{orderId}/pickup")]
		public async Task<ActionResult> Pickup(Guid orderId)
		{
			// TODO: Ensure user has Trader role
			// TODO: Retrieve ownerId from user context
			var ownerId = Guid.NewGuid();
			var command = new PickupOrderCommand(orderId, ownerId);
			await _queueClient.Send(_configuration.CommandQueueName, command);

			return Ok();
		}

		/// <summary>
		/// Request put down of an order
		/// </summary>
		/// <param name="orderId">The id of the order to put down</param>
		/// <returns>200 OK if the request was successfully processed.</returns>
		[HttpPost("{orderId}/putdown")]
		public async Task<ActionResult<Order>> PutDown(Guid orderId)
		{
			// TODO: Ensure user has Trader role
			// TODO: Validate that the user requesting the put down is the user who picked it up.
			var command = new PutDownOrderCommand(orderId);
			await _queueClient.Send(_configuration.CommandQueueName, command);

			return Ok();
		}

		/// <summary>
		/// Retrieve all orders
		/// </summary>
		/// <returns>All orders present in the system</returns>
		[HttpGet]
		public IActionResult Get()
		{
			var orders = _orderReader.Get();

			return Ok(orders);
		}
	}
}
