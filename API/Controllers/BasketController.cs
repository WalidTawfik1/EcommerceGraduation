using AutoMapper;
using Ecom.API.Helper;
using Ecom.Core.Entities;
using Ecom.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecom.API.Controllers
{
    public class BasketController : BaseController
    {
        public BasketController(IUnitofWork work, IMapper mapper) : base(work, mapper)
        {
        }

        [HttpGet("GetBasketById/{id}")]
        public async Task<ActionResult> GetBasketById(string id)
        {
            var result = await work.CustomerBasketRepository.GetBasketAsync(id);
            return Ok(result ?? new CustomerBasket());
        }

        [HttpPost("UpdateBasket")]
        public async Task<ActionResult> UpdateBasket(CustomerBasket basket)
        {
            var result = await work.CustomerBasketRepository.UpdateBasketAsync(basket);
            return Ok(result);
        }

        [HttpDelete("DeleteBasket/{id}")]
        public async Task<ActionResult> DeleteBasket(string id)
        {
            var result = await work.CustomerBasketRepository.DeleteBasketAsync(id);
            return result ? Ok(new APIResponse(200,"Basket Deleted Succesfully"))
                : BadRequest(new APIResponse(400,"Failed to Delete Basket"));
        }
    }
}
