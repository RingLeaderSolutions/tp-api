using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theta.Platform.Messaging.Commands;

namespace Theta.Platform.RFQ.Management.Service.Domain.Commands
{
    public class CancelRFQCommand : Command
    {
        public Guid RFQIdentitier { get; set; }
    }
}
