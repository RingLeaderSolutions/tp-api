namespace Theta.Platform.UI.Pricing.Streaming.Domain.Notifications
{
    public class SystemNotification : Notification
    {
        public string Message { get; set; }


        public SystemNotification()
        {
            Type = NotificationType.SYSTEM_NOTIFICATION;
        }
    }
}