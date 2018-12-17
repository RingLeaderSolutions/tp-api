using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Theta.Paltform.Order.Read.Service
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TimeInForce
    {
        GoodTillDay,
        GoodTillDate,
        GoodTillCancel
    }
}