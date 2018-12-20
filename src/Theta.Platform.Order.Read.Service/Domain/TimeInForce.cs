using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Theta.Platform.Order.Read.Service.Domain
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TimeInForce
    {
        GoodTillDay,
        GoodTillDate,
        GoodTillCancel
    }
}