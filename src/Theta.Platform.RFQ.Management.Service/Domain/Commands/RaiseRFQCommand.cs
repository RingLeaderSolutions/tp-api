// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
using System;
using System.Collections.Generic;
using Theta.Platform.Messaging.Commands;

namespace Theta.Platform.RFQ.Management.Service.Domain.Commands
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
