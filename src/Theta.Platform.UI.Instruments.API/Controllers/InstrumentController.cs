using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Theta.Platform.Domain.Instruments;

namespace Theta.Platform.UI.Instruments.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstrumentController : ControllerBase
    {
        public List<Instrument> Instruments => InstrumentSampleData.All;

        [HttpGet]
        public ActionResult<IEnumerable<Instrument>> Get()
        {
            return Ok(Instruments);
        }

        [HttpGet("{id}")]
        public ActionResult<Instrument> Get(string id)
        {
            var foundInstrument = Instruments
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
