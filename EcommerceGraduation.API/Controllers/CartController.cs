using AutoMapper;
using EcommerceGraduation.API.Helper;
using EcommerceGraduation.Core.Entities;
using EcommerceGraduation.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceGraduation.API.Controllers
{
    /// <summary>
    /// Controller for managing shopping carts.
    /// </summary>
    public class CartController : BaseController
    {
        public CartController(IUnitofWork work, IMapper mapper) : base(work, mapper)
        {
        }

        /// <summary>
        /// Gets a shopping cart by its ID.
        /// </summary>
        /// <param name="id">The cart ID.</param>
        /// <returns>The shopping cart with the specified ID.</returns>
        [HttpGet("GetBasketById/{id}")]
        public async Task<ActionResult> GetBasketById(string id)
        {
            var result = await work.CartRepository.GetCartAsync(id);
            return Ok(result ?? new Cart());
        }

        /// <summary>
        /// Updates a shopping cart.
        /// </summary>
        /// <param name="basket">The cart details.</param>
        /// <returns>The updated shopping cart.</returns>
        [HttpPost("UpdateBasket")]
        public async Task<ActionResult> UpdateBasket(Cart basket)
        {
            var result = await work.CartRepository.UpdateCartAsync(basket);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a shopping cart by its ID.
        /// </summary>
        /// <param name="id">The cart ID.</param>
        /// <returns>A response indicating the result of the deletion.</returns>
        [HttpDelete("DeleteBasket/{id}")]
        public async Task<ActionResult> DeleteBasket(string id)
        {
            var result = await work.CartRepository.DeleteCartAsync(id);
            return result ? Ok(new APIResponse(200, "Basket Deleted Successfully"))
                : BadRequest(new APIResponse(400, "Failed to Delete Basket"));
        }
    }
}
