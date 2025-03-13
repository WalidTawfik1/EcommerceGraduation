using AutoMapper;
using Ecom.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecom.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected readonly IUnitofWork work;
        protected readonly IMapper mapper;

        public BaseController(IUnitofWork work,IMapper mapper)
        {
            this.work = work;
            this.mapper = mapper;
        }
    }
}
