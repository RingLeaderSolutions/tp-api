using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Theta.Platform.Order.Management.Service
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
        Done
    }
}