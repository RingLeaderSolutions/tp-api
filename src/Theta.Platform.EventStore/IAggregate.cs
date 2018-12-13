using System;
using System.Collections.Generic;
using System.Text;

namespace Theta.Platform.EventStore
{
    public interface IAggregate
    {
        int Version { get; }
        object Identifier { get; }
        void ApplyEvent(object @event);
        ICollection<object> GetPendingEvents();
        void ClearPendingEvents();
    }
}
