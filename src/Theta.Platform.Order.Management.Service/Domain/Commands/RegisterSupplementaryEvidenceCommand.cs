using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Theta.Platform.Order.Management.Service.Domain.Commands
{
    public class RegisterSupplementaryEvidenceCommand
    {
        public Guid OrderId { get; set; }
        public string SupplementaryEvidence { get; set; }
    }
}
