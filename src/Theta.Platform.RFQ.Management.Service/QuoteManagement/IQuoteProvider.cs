using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Theta.Platform.RFQ.Management.Service.QuoteManagement
{
    public interface IQuoteProvider
    {
        void CancelRFQ(Guid rfqIdentifier);

        void SubmitRFQ(Guid rfqIdentifier, Guid orderId, List<string> counterParties, Guid instrument, decimal originalPrice);
    }
}