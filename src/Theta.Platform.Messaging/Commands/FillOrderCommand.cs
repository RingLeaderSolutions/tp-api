using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Theta.Platform.Messaging.Commands
{
    public class FillOrderCommand : Command
    {
        public Guid OrderId { get; set; }

        public Guid RFQId { get; set; }

        public decimal Quantity { get; set; }

        public decimal Price { get; set; }
    }
}
