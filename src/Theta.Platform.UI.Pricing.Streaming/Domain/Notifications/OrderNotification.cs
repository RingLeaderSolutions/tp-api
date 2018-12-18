namespace Theta.Platform.UI.Pricing.Streaming.Domain.Notifications
{
    public class OrderNotification : Notification
    {
        public string OrderId { get; set; }

        public string Event { get; set; }

        public string Time { get; set; }

        public string Operation { get; set; }

        public string AuditInfo { get; set; }
    }
}