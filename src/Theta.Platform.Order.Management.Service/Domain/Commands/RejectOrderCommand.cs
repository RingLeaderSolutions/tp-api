using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Theta.Platform.Order.Management.Service.Domain.Commands
{
    public class RejectOrderCommand
    {
        public Guid OrderId { get; set; }
        public string Reason { get; set; }
    }
}
