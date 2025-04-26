using AutoMapper;
using EcommerceGraduation.API.Helper;
using EcommerceGraduation.Core.Entities;
using EcommerceGraduation.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceGraduation.API.Controllers
{

    public class WishlistController : BaseController
    {
        public WishlistController(IUnitofWork work, IMapper mapper) : base(work, mapper)
        {
        }

        /// <summary>
        /// Adds an item to a wishlist.
        /// </summary>
        /// <param name="wishlistId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("add-to-wishlist")]
        public async Task<ActionResult> AddToCart(string wishlistId, [FromBody] AddtoWhishlist request)
        {
            if (string.IsNullOrEmpty(wishlistId) || request == null)
            {
                return BadRequest(new APIResponse(400, "Invalid parameters"));
            }

            var result = await work.WishlistRepository.AddToWishlist(wishlistId, request.ProductId);
            if (result == null)
            {
                return BadRequest(new APIResponse(400, "Failed to add item to wishlist"));
            }

            return Ok(result);
        }

        /// <summary>
        /// Gets a wishlist by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("get-wishlist-by-id/{id}")]
        public async Task<ActionResult> GetWishlistById(string id)
        {
            var result = await work.WishlistRepository.GetWishlist(id);
            return Ok(result ?? new Wishlist());
        }

        /// <summary>
        /// Deletes a wishlist by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpDelete("clear-wishlist/{id}")]
        public async Task<ActionResult> ClearWishlist(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new APIResponse(400, "Invalid parameters"));
            }
            var result = await work.WishlistRepository.ClearWishlist(id);
            if (!result)
            {
                return BadRequest(new APIResponse(400, "Failed to clear wishlist"));
            }
            return Ok(new APIResponse(200, "Wishlist cleared successfully"));
        }

        /// <summary>
        /// Removes an item from a wishlist.
        /// </summary>
        /// <param name="wishlistId"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpDelete("remove-item-from-wishlist")]
        public async Task<ActionResult> RemoveItemFromWishlist(string wishlistId, int productId)
        {
            if (string.IsNullOrEmpty(wishlistId) || productId <= 0)
            {
                return BadRequest(new APIResponse(400, "Invalid parameters"));
            }
            var result = await work.WishlistRepository.RemoveFromWishlist(wishlistId, productId);
            if (!result)
            {
                return BadRequest(new APIResponse(400, "Failed to remove item from wishlist"));
            }
            return Ok(new APIResponse(200, "Item removed from wishlist successfully"));
        }
    }
}
