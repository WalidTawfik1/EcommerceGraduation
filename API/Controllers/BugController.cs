using AutoMapper;
using Ecom.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecom.API.Controllers
{

    public class BugController : BaseController
    {
        public BugController(IUnitofWork work, IMapper mapper) : base(work, mapper)
        {
        }
        [HttpGet("not-found")]
        public async Task<IActionResult> GetNotFound()
        {
            var category = await work.CategoryRepository.GetByIdAsync(100);
            if (category == null) return NotFound();
            return Ok(category);
        }
        [HttpGet("server-error")]
        public async Task<IActionResult> GetServerError()
        {
            var category = await work.CategoryRepository.GetByIdAsync(100);
            category.Name = "";
            return Ok(category);
        }
        [HttpGet("bad-request/{Id}")]
        public async Task<IActionResult> GetBadRequest(int Id)
        {
            return Ok();
        }
        [HttpGet("bad-request")]
        public async Task<IActionResult> GetBadRequest()
        {
            return BadRequest();
        }

    }
}
