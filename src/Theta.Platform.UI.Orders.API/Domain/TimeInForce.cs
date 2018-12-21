using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Theta.Platform.UI.Orders.API.Domain
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TimeInForce
    {
        GoodTillDay,
        GoodTillDate,
        GoodTillCancel
    }
}