using System;
using System.Collections.Generic;
using System.Linq;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.Domain
{
	public abstract class AggregateRoot : IAggregateRoot
	{
		readonly Dictionary<Type, Action<object>> _handlers = new Dictionary<Type, Action<object>>();

		readonly List<IEvent> _events = new List<IEvent>();

		public Guid Id { get; protected set; }

		public int Version { get; protected set; } = -1;

        public List<IEvent> GetEvents()
		{
			return _events;
		}
		
		protected void Register<T>(Action<T> when)
		{
			_handlers.Add(typeof(T), e => when((T)e));
		}

		protected void Raise(IEvent e)
		{
			_handlers[e.GetType()](e);
			_events.Add(e);
		}

        public void Apply(IEvent e)
        {
			// TODO: Ideally this check shouldn't ever have to happen, but as we maintain a subscription across multiple
			// streams, at present we get duplicates, e.g. $event1 from stream $order-{id}, $event1 from stream $et-event1, $event1 from stream $all, etc...
	        if (_events.SingleOrDefault(ev => ev.EventId == e.EventId) != null)
	        {
				// TODO: Log the fact we received a duplicate event?
				return;
	        }

            Raise(e);
            Version++;
        }
    }
}