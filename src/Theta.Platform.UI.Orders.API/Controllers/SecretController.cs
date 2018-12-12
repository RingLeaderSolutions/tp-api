using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Theta.Platform.Common.SecretManagement;

namespace Theta.Platform.UI.Orders.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecretController : ControllerBase
    {
        private readonly IVault _vault;

        public SecretController(IVault vault)
        {
            _vault = vault;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            // Note - this is just a proof of concept - we should not call the key vault per action like this due to rate limiting
            var secretValue = await _vault.GetSecret("testSecret");

            return new OkObjectResult(new { secretValue = secretValue });
        }
    }
}
