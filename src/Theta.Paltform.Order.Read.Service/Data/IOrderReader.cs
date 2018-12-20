using System;
using System.Threading.Tasks;

namespace Theta.Platform.Order.Read.Service.Data
{
	public interface IOrderReader
	{
		Task StartAsync();

		Domain.Order[] Get();

		Domain.Order GetById(Guid id);
	}
}