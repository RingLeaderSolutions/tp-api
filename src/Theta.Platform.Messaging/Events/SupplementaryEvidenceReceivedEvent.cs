using System;

namespace Theta.Platform.Messaging.Events
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
