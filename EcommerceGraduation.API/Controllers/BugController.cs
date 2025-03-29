using AutoMapper;
using EcommerceGraduation.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceGraduation.API.Controllers
{
    /// <summary>
    /// Controller for testing and handling bugs.
    /// </summary>
    public class BugController : BaseController
    {
        public BugController(IUnitofWork work, IMapper mapper) : base(work, mapper)
        {
        }

        /// <summary>
        /// Simulates a not found error.
        /// </summary>
        /// <returns>A not found response.</returns>
        [HttpGet("not-found")]
        public async Task<IActionResult> GetNotFound()
        {
            var category = await work.CategoryRepository.GetByIdAsync("100");
            if (category == null) return NotFound();
            return Ok(category);
        }

        /// <summary>
        /// Simulates a server error.
        /// </summary>
        /// <returns>A server error response.</returns>
        [HttpGet("server-error")]
        public async Task<IActionResult> GetServerError()
        {
            var category = await work.CategoryRepository.GetByIdAsync("100");
            category.Name = "";
            return Ok(category);
        }

        /// <summary>
        /// Simulates a bad request error with an ID.
        /// </summary>
        /// <param name="Id">The ID.</param>
        /// <returns>A bad request response.</returns>
        [HttpGet("bad-request/{Id}")]
        public async Task<IActionResult> GetBadRequest(int Id)
        {
            return Ok();
        }

        /// <summary>
        /// Simulates a bad request error.
        /// </summary>
        /// <returns>A bad request response.</returns>
        [HttpGet("bad-request")]
        public async Task<IActionResult> GetBadRequest()
        {
            return BadRequest();
        }
    }
}
