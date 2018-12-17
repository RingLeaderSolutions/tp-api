using System;
using System.Collections.Generic;

namespace Theta.Paltform.Order.Read.Service.Framework
{
    public interface IAggregateRoot
    {
        List<object> GetEvents();

        void ClearEvents();

        void Apply(object e);

        Guid Id { get; }

        int Version { get; }
    }
}