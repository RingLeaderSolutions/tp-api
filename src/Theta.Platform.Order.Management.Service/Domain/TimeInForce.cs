using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Theta.Platform.Order.Management.Service
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TimeInForce
    {
        GoodTillDay,
        GoodTillDate,
        GoodTillCancel
    }
}