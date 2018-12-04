using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Theta.Platform.UI.Instruments.API.Model;

namespace Theta.Platform.UI.Instruments.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstrumentController : ControllerBase
    {
        private List<Instrument> Instruments = new List<Instrument>
        {
            new Instrument
            {
                Id = Guid.NewGuid(),
                AssetClass = AssetClassType.FixedIncome,
                Ids = new List<InstrumentId>
                {
                    new InstrumentId { Id = "GB00B16NNR78", IdType = InstrumentIdType.ISIN },
                    new InstrumentId { Id = "0088279A7", IdType = InstrumentIdType.CUSIP },
                    new InstrumentId { Id = "B16NNR7", IdType = InstrumentIdType.SEDOL },
                    new InstrumentId { Id = "A0GZPU", IdType = InstrumentIdType.WKN },
                    new InstrumentId { Id = "TR27", IdType = InstrumentIdType.Symbol },
                }.ToArray(),
                Name = "UK 10 Year Gilt",
                Category = "Government Bond",
                CouponRate = 4.250m,
                CouponType = CouponType.Fixed,
                CouponFrequency = CouponFrequency.SemiAnnually,
                AccruedInterest = 1.87m,
                AccruedDays = 160,
                IssueDate = new DateTimeOffset(2006, 09, 06, 0, 0, 0, TimeSpan.Zero),
                MaturityDate = new DateTimeOffset(2027, 12, 07, 0, 0, 0, TimeSpan.Zero),
                CouponPaymentDate = new DateTimeOffset(2018, 12, 07, 0, 0, 0, TimeSpan.Zero),
                MinimumDenomination = 0.01m,
                DenominationCcy = "GBP"
            },
            new Instrument
            {
                Id = Guid.NewGuid(),
                AssetClass = AssetClassType.FixedIncome,
                Ids = new List<InstrumentId>
                {
                    new InstrumentId { Id = "XS0158715713", IdType = InstrumentIdType.ISIN },
                    new InstrumentId { Id = "EC7667958", IdType = InstrumentIdType.CUSIP },
                    new InstrumentId { Id = "3230097", IdType = InstrumentIdType.SEDOL },
                    new InstrumentId { Id = "249002", IdType = InstrumentIdType.WKN },
                    new InstrumentId { Id = "VO32", IdType = InstrumentIdType.Symbol },
                }.ToArray(),
                Name = "Vodafone Group Plc",
                Category = "Corporate Bond",
                CouponRate = 5.900m,
                CouponType = CouponType.Fixed,
                CouponFrequency = CouponFrequency.Annually,
                AccruedInterest = 0m,
                AccruedDays = 0,
                IssueDate = new DateTimeOffset(2002, 11, 02, 0, 0, 0, TimeSpan.Zero),
                MaturityDate = new DateTimeOffset(2032, 11, 26, 0, 0, 0, TimeSpan.Zero),
                CouponPaymentDate = new DateTimeOffset(2018, 11, 26, 0, 0, 0, TimeSpan.Zero),
                MinimumDenomination = 1000,
                DenominationCcy = "GBP"
            }
        };

        [HttpGet]
        public ActionResult<IEnumerable<Instrument>> Get()
        {
            return this.Instruments;
        }

        [HttpGet("{id}")]
        public ActionResult<Instrument> Get(string id)
        {
            var foundInstrument = this.Instruments
                .SingleOrDefault(i =>
                {
                    if (i.Id.ToString() == id)
                    {
                        return true;
                    }

                    return i.Ids.SingleOrDefault(identifier => identifier.Id == id) != null;
                });

            if (foundInstrument != null)
            {
                return Ok(foundInstrument);
            }
            return NotFound();
        }

        [HttpPost]
        public ActionResult Post([FromBody] Instrument value)
        {
            var instrumentId = Guid.NewGuid();
            value.Id = instrumentId;
            this.Instruments.Add(value);
            return Created($"/api/instrument/{instrumentId.ToString()}", instrumentId.ToString());
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Instrument value)
        {
            throw new NotSupportedException();
        }

        [HttpDelete("{id}")]
        public void Delete(Guid id)
        {
            throw new NotSupportedException();
        }
    }
}
