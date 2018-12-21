// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
using System;
using Theta.Platform.Messaging.Commands;

namespace Theta.Platform.Order.Seed.Console.Commands
{
    public sealed class RegisterSupplementaryEvidenceCommand : Command
    {
	    public RegisterSupplementaryEvidenceCommand(Guid orderId, string supplementaryEvidence)
	    {
		    OrderId = orderId;
		    SupplementaryEvidence = supplementaryEvidence;
	    }

	    public Guid OrderId { get; }

        public string SupplementaryEvidence { get; }
    }
}
