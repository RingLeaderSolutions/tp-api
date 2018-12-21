// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
using System;
using Theta.Platform.Messaging.Commands;

namespace Theta.Platform.RFQ.Management.Service.Domain.Commands
{
    public class CancelRFQCommand : Command
    {
        public Guid RFQIdentitier { get; set; }
    }
}
