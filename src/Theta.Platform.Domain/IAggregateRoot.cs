using System;
using System.Collections.Generic;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.Domain
{
	public interface IAggregateRoot
	{
		void Apply(IEvent e);

		Guid Id { get; }

		int Version { get; }
	}
}