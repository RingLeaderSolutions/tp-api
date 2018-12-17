using System;

namespace Theta.Paltform.Order.Read.Service.Framework
{
    public class DomainException : Exception
    {
        public DomainException(string message)
            :base(message)
        {
        }
    }
}