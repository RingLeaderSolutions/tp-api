using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Theta.Platform.Order.Read.Service.Domain
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OrderStatus
    {
        New,
        Pending,
        Accepted,
        Working,
        PartiallyFilled,
        Filled,
        Done, 
        Rejected
    }
}