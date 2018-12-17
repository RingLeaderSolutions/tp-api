using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Theta.Paltform.Order.Read.Service
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