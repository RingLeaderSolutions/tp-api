using System;
using System.Threading.Tasks;
using Theta.Platform.Domain;
using Theta.Platform.UI.Orders.API.Domain;

namespace Theta.Platform.UI.Orders.API.Services
{
	public interface IOrderReader : IAggregateReader<Order>
	{
		Task StartAsync();

		Order[] Get();

		Order GetById(Guid id);
	}
}