using System;
using Theta.Platform.Messaging.Commands;

namespace Theta.Platform.Order.Seed.Console.Commands
{
    public class RegisterSupplementaryEvidenceCommand : Command
    {
        public Guid OrderId { get; set; }

        public string SupplementaryEvidence { get; set; }
    }
}
