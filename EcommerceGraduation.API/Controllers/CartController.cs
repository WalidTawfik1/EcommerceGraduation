using AutoMapper;
using EcommerceGraduation.API.Helper;
using EcommerceGraduation.Core.Entities;
using EcommerceGraduation.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
        [AllowAnonymous]
        [HttpGet("get-cart-by-id/{id}")]
        public async Task<ActionResult> GetBasketById(string id)
        {
            var result = await work.CartRepository.GetCartAsync(id);
            return Ok(result ?? new Cart());
        }

        /// <summary>
        /// Adds an item to a shopping cart.
        /// </summary>
        /// <param name="cartId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("add-to-cart")]
        public async Task<ActionResult> AddToCart(string cartId, [FromBody] AddtoCart request)
        {
            if (string.IsNullOrEmpty(cartId) || request == null)
            {
                return BadRequest(new APIResponse(400, "Invalid parameters"));
            }

            var result = await work.CartRepository.AddToCartAsync(cartId, request.ProductId, request.Quantity);
            if (result == null)
            {
                return BadRequest(new APIResponse(400, "Failed to add item to cart"));
            }

            return Ok(result);
        }

        /// <summary>
        /// Deletes a shopping cart by its ID.
        /// </summary>
        /// <param name="id">The cart ID.</param>
        /// <returns>A response indicating the result of the deletion.</returns>
        [AllowAnonymous]
        [HttpDelete("delete-cart/{id}")]
        public async Task<ActionResult> DeleteBasket(string id)
        {
            var result = await work.CartRepository.DeleteCartAsync(id);
            return result ? Ok(new APIResponse(200, "Basket Deleted Successfully"))
                : BadRequest(new APIResponse(400, "Failed to Delete Basket"));
        }

        /// <summary>
        /// Removes an item from a shopping cart.
        /// </summary>
        /// <param name="cartId">The cart ID.</param>
        /// <param name="productId">The product ID to remove.</param>
        /// <returns>The updated shopping cart.</returns>
        [AllowAnonymous]
        [HttpDelete("remove-item")]
        public async Task<ActionResult> RemoveItemFromCart(string cartId, int productId)
        {
            if (string.IsNullOrEmpty(cartId) || productId <= 0)
            {
                return BadRequest(new APIResponse(400, "Invalid parameters"));
            }

            var result = await work.CartRepository.RemoveItemFromCartAsync(cartId, productId);
            if (result == null)
            {
                return BadRequest(new APIResponse(400, "Failed to remove item from cart"));
            }

            return Ok(result);
        }

        /// <summary>
        /// Updates the quantity of an item in a shopping cart.
        /// </summary>
        /// <param name="cartId">The cart ID.</param>
        /// <param name="productId">The product ID to update.</param>
        /// <param name="quantity">The new quantity.</param>
        /// <returns>The updated shopping cart.</returns>
        [AllowAnonymous]
        [HttpPut("update-quantity")]
        public async Task<ActionResult> UpdateItemQuantity(string cartId, int productId, int quantity)
        {
            if (string.IsNullOrEmpty(cartId) || productId <= 0 || quantity < 0)
            {
                return BadRequest(new APIResponse(400, "Invalid parameters"));
            }

            var result = await work.CartRepository.UpdateItemQuantityAsync(cartId, productId, quantity);
            if (result == null)
            {
                return BadRequest(new APIResponse(400, "Failed to update item quantity"));
            }

            return Ok(result);
        }


    }
}
