using System;
using System.Collections.Generic;
using System.Text;
using Theta.Platform.Messaging.Commands;

namespace Theta.Platform.Order.Seed.Console.Commands
{
    public class RaiseRFQCommand : Command
    {
        public Guid RFQIdentitier { get; set; }

        public Guid OrderId { get; set; }

        public Guid Instrument { get; set; }

        public DateTimeOffset Requested { get; set; }

        public List<string> CounterParties { get; set; }
    }
}
