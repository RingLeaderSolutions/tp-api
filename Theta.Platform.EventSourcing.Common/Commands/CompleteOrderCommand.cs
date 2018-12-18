using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Theta.Platform.Messaging.Commands
{
    public class CompleteOrderCommand
    {
        public Guid OrderId { get; set; }
    }
}
