using System;

namespace Theta.Platform.Order.Management.Service
{
    public class TimeInForce
    {
        public TimeInForceType Type { get; set; }

        // "TIF Date" "TIF Time"
        public DateTimeOffset Expiry { get; set; }

        public enum TimeInForceType
        {
            Day,
            GoodTillDate
        }
    }
}