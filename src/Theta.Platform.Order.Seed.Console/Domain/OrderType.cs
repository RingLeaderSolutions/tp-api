using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Theta.Platform.Order.Seed.Console.Domain
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OrderType
    {
        Market,
        Limit
    }
}