using AutoMapper;
using EcommerceGraduation.API.Helper;
using EcommerceGraduation.Core.Entities;
using EcommerceGraduation.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceGraduation.API.Controllers
{
    public class CartController : BaseController
    {
        public CartController(IUnitofWork work, IMapper mapper) : base(work, mapper)
        {
        }
        [HttpGet("GetBasketById/{id}")]
        public async Task<ActionResult> GetBasketById(string id)
        {
            var result = await work.CartRepository.GetCartAsync(id);
            return Ok(result ?? new Cart());
        }

        [HttpPost("UpdateBasket")]
        public async Task<ActionResult> UpdateBasket(Cart basket)
        {
            var result = await work.CartRepository.UpdateCartAsync(basket);
            return Ok(result);
        }

        [HttpDelete("DeleteBasket/{id}")]
        public async Task<ActionResult> DeleteBasket(string id)
        {
            var result = await work.CartRepository.DeleteCartAsync(id);
            return result ? Ok(new APIResponse(200, "Basket Deleted Succesfully"))
                : BadRequest(new APIResponse(400, "Failed to Delete Basket"));
        }
    }
}
