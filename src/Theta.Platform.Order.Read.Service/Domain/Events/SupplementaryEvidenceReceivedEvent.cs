using System;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.Order.Read.Service.Domain.Events
{
    public class SupplementaryEvidenceReceivedEvent : Event
    {
        public SupplementaryEvidenceReceivedEvent(Guid orderId, string supplementaryEvidence)
			: base(orderId)
        {
            SupplementaryEvidence = supplementaryEvidence;
        }

		public Guid OrderId => AggregateId;

		public string SupplementaryEvidence { get; }
    }
}
