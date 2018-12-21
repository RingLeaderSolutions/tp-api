﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theta.Platform.Messaging.Commands;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.RFQ.Management.Service.Messaging.Subscribers
{
    public interface ISubscriber<T, V> where T : ICommand where V : IEvent
    {
        Type CommandType { get; }

        Task HandleCommand(IActionableMessage<ICommand> command);
    }
}