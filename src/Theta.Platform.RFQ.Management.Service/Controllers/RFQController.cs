using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Theta.Platform.RFQ.Management.Service.Domain.Commands;
using Theta.Platform.RFQ.Management.Service.QuoteManagement;

namespace Theta.Platform.RFQ.Management.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RFQController : ControllerBase
    {
        private readonly IQuoteProvider _quoteProvider;

        public RFQController(IQuoteProvider quoteProvider)
        {
            _quoteProvider = quoteProvider;
        }

        [HttpPost("{rfqIdentifier}")]
        public void Post([FromBody]RaiseRFQCommand rFQRequestCommand)
        {
            _quoteProvider.SubmitRFQ(
                rFQRequestCommand.RFQIdentitier,
                rFQRequestCommand.OrderId,
                rFQRequestCommand.CounterParties, 
                rFQRequestCommand.Instrument, 
                125.12M);
        }

        // DELETE api/values/5
        [HttpPost("Cancel/{rfqIdentifier}")]
        public void Cancel(Guid rfqIdentifier)
        {
            _quoteProvider.CancelRFQ(rfqIdentifier);
        }
    }
}
