namespace Theta.Platform.Order.Management.Service.Domain
{
    public class InvalidStateChangeException
    {
        public InvalidStateChangeException(string eventType, string reason)
        {
            EventType = eventType;

            Reason = reason;
        }

        public string EventType { get; }

        public string Reason { get; }
    }
}
