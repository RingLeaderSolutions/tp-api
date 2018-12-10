using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace Theta.Platform.UI.Orders.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyVersion = assembly.GetName().Version;

            return new OkObjectResult(new { version = assemblyVersion.ToString() });
        }
    }
}
