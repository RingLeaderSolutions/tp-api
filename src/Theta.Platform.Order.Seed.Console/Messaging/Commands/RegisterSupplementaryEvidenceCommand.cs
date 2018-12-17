using System;
using System.Collections.Generic;
using System.Text;

namespace Theta.Platform.Order.Seed.Console.Messaging.Commands
{
    public class RegisterSupplementaryEvidenceCommand
    {
        public Guid OrderId { get; }
        public string SupplementaryEvidence { get; }
    }
}
