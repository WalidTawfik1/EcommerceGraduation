using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ecom.API.Helper;

namespace Ecom.API.Controllers
{
    [Route("errors/{statusCode}")]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [HttpGet]
        public IActionResult Error(int statusCode)
        {
            return new ObjectResult(new APIResponse(statusCode));
        }
    }
}
