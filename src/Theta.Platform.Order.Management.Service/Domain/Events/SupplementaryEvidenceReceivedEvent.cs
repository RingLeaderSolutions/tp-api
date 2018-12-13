using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Theta.Platform.Order.Management.Service.Domain.Events
{
    public class SupplementaryEvidenceReceivedEvent
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
