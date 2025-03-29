using EcommerceGraduation.API.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceGraduation.API.Controllers
{
    /// <summary>
    /// Controller for handling errors.
    /// </summary>
    [Route("errors/{statusCode}")]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        /// <summary>
        /// Handles errors based on the status code.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <returns>An error response.</returns>
        [HttpGet]
        public IActionResult Error(int statusCode)
        {
            return new ObjectResult(new APIResponse(statusCode));
        }
    }
}
