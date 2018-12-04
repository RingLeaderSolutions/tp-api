namespace Theta.Platform.Domain
{
    public enum OrderStatus
    {
        New,
        Pending,
        Accepted,
        Working,
        PartiallyFilled,
        Filled,
        Done
    }
}