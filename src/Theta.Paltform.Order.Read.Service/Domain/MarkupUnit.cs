using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Theta.Paltform.Order.Read.Service
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MarkupUnit
    {
        PercentageOfWhole,
        BasisPoints,
        ActualValue
    }
}
