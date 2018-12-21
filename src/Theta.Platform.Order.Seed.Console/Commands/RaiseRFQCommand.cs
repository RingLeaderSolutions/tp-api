// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
using System;
using System.Collections.Generic;
using Theta.Platform.Messaging.Commands;

namespace Theta.Platform.Order.Seed.Console.Commands
{
    public sealed class RaiseRFQCommand : Command
    {
	    public RaiseRFQCommand(Guid rfqIdentitier, Guid orderId, Guid instrument, DateTimeOffset requested, List<string> counterParties)
	    {
		    RFQIdentitier = rfqIdentitier;
		    OrderId = orderId;
		    Instrument = instrument;
		    Requested = requested;
		    CounterParties = counterParties;
	    }

	    public Guid RFQIdentitier { get; }

        public Guid OrderId { get; }

        public Guid Instrument { get; }

        public DateTimeOffset Requested { get; }

        public List<string> CounterParties { get; }
    }
}
