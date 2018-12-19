using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.Messaging.Events
{
    public class SupplementaryEvidenceReceivedEvent : IEvent
    {
        public SupplementaryEvidenceReceivedEvent(Guid orderId, string supplementaryEvidence)
        {
            OrderId = orderId;
            SupplementaryEvidence = supplementaryEvidence;
        }

        public Guid OrderId { get; }

        public string SupplementaryEvidence { get; }

        public Guid AggregateId => OrderId;

        public string Type => this.GetType().Name;
    }
}
