using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Theta.Paltform.Order.Read.Service.Domain.Events
{
    public class SupplementaryEvidenceReceivedEvent : IDomainEvent
    {
        public SupplementaryEvidenceReceivedEvent(Guid orderId, string supplementaryEvidence)
        {
            OrderId = orderId;
            SupplementaryEvidence = supplementaryEvidence;
        }

        public Guid OrderId { get; }
        public string SupplementaryEvidence { get; }
    }
}
