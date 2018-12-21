using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Theta.Platform.UI.Orders.API.Services
{
	public sealed class OrderService : IHostedService
	{
		private readonly IOrderReader _orderReader;

		public OrderService(IOrderReader orderReader)
		{
			_orderReader = orderReader;
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			await _orderReader.StartAsync();
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			_orderReader.Dispose();
			return Task.CompletedTask;
		}
	}
}