﻿using System.Threading.Tasks;
using Theta.Platform.Domain;
using Theta.Platform.Messaging.Commands;
using Theta.Platform.Messaging.Events;
using Theta.Platform.Order.Management.Service.Domain.Commands;
using Theta.Platform.Order.Management.Service.Domain.Events;

namespace Theta.Platform.Order.Management.Service.Messaging.Subscribers
{
    public class FillOrderSubscriber : Subscriber<FillOrderCommand, OrderFilledEvent>, ISubscriber<FillOrderCommand, OrderFilledEvent>
    {
        public FillOrderSubscriber(IAggregateWriter<Domain.Aggregate.Order> aggregateWriter) : base(aggregateWriter)
        {
        }

        protected override async Task<OrderFilledEvent> Handle(FillOrderCommand command)
        {
            var order = AggregateWriter.GetById(command.OrderId);

            // Check order state

            var orderFilldEvent = new OrderFilledEvent(command.OrderId, command.RFQId, command.Price, command.Quantity);

            return orderFilldEvent;
        }
    }
}