using System.Threading.Tasks;
using Theta.Platform.Domain;
using Theta.Platform.Messaging.Commands;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.Order.Management.Service.Messaging.Subscribers
{
    public class RegisterSupplementaryEvidenceSubscriber : Subscriber<RegisterSupplementaryEvidenceCommand, SupplementaryEvidenceReceivedEvent>, ISubscriber<RegisterSupplementaryEvidenceCommand, SupplementaryEvidenceReceivedEvent>
    {
        public RegisterSupplementaryEvidenceSubscriber(IAggregateWriter<Domain.Order> aggregateWriter) : base(aggregateWriter)
        {
        }

        protected override async Task<SupplementaryEvidenceReceivedEvent> Handle(RegisterSupplementaryEvidenceCommand command)
        {
            var order = AggregateWriter.GetById(command.OrderId);

            // Check order state

            var supplementaryEvidenceReceivedEvent = new SupplementaryEvidenceReceivedEvent(command.OrderId, command.SupplementaryEvidence);

            return supplementaryEvidenceReceivedEvent;
        }
    }
}
