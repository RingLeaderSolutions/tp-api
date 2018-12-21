using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Theta.Platform.Order.Management.Service.Domain.Aggregate
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum Side
	{
		Buy,
		Sell
	}
}