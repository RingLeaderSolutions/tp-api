using System;

namespace Theta.Platform.Order.Management.Service.Framework
{
    public class DomainException : Exception
    {
        public DomainException(string message)
            :base(message)
        {
        }
    }
}