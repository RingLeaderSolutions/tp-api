using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Theta.Platform.UI.Pricing.Streaming.Domain.Notifications
{
    public class Notification
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public NotificationType Type { get; set; }

        public string ReceivedAt { get; set; }
    }
}