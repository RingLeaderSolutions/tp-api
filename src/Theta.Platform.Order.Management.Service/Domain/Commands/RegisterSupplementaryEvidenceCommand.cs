using System;
using Theta.Platform.Messaging.Commands;

namespace Theta.Platform.Order.Management.Service.Domain.Commands
{
    public class RegisterSupplementaryEvidenceCommand : Command
    {
        public Guid OrderId { get; set; }

        public string SupplementaryEvidence { get; set; }
    }
}
