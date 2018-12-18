using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Theta.Platform.UI.Orders.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnvironmentController : ControllerBase
    {
        [HttpGet]
        public ActionResult Get()
        {
            // Just a POC
            // If we can retrieve an environment name then we should be able to determine all conventionally named
            // usernames, connection strings, other Azure resources etc. and just be left with secret management

            var environmentName = Environment.GetEnvironmentVariable("theta-environment");

            return new OkObjectResult(new { environmentName = environmentName });
        }
    }
}
