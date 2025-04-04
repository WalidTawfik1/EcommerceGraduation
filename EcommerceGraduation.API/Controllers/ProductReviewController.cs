using EcommerceGraduation.API.Helper;
using EcommerceGraduation.Core.DTO;
using EcommerceGraduation.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcommerceGraduation.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductReviewController : ControllerBase
    {
        private readonly IProductReview _productReview;
        public ProductReviewController(IProductReview productReview)
        {
            _productReview = productReview;
        }
        /// <summary>
        /// Add a review for a product, Rating should be from 1 to 5.
        /// </summary>
        /// <param name="reviewDTO"></param>
        [Authorize]
        [HttpPost("add-product-review")]
        public async Task<IActionResult> AddRating([FromBody] ProductReviewDTO reviewDTO)
        {
            if (reviewDTO == null)
            {
                return BadRequest(new APIResponse(400, "Invalid review data."));
            }

            try
            {
                var customerCode = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(customerCode))
                {
                    return Unauthorized(new APIResponse(401, "Please login or register."));
                }

                var result = await _productReview.AddRatingAsync(reviewDTO, customerCode);

                if (result)
                {
                    return Ok(new APIResponse(200, "Review added successfully."));
                }

                return BadRequest(new APIResponse(400, "Failed to add review. Please try again, or user already added a review before."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new APIResponse(500, $"An unexpected error occurred,{ex.Message}."));
            }
        }


        /// <summary>
        /// Get all reviews for a product.
        /// </summary>
        /// <param name="productId"></param>
        /// <returns>A list of product reviews.</returns>
        [AllowAnonymous]
        [HttpGet("get-product-review/{productId}")]
        public async Task<IActionResult> GetAllRatingForProduct(int productId)
        {
            var result = await _productReview.GetAllRatingForProductAsync(productId);
            return Ok(result);
        }
        /// <summary>
        /// Update a review for a product.
        /// </summary>
        /// <param name="reviewDTO"></param>
        [Authorize]
        [HttpPut("update-product-review")]
        public async Task<IActionResult> UpdateReview([FromBody] UpdateReviewDTO reviewDTO)
        {
            var customerCode = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(customerCode))
            {
                return Unauthorized(new APIResponse(401, "Please Login or Register"));
            }

            var result = await _productReview.UpdateReviewAsync(reviewDTO, customerCode);
            return result ? Ok(new APIResponse(200, "Review Updated Successfully"))
                          : NotFound(new APIResponse(404, "Review Not Found or Unauthorized"));
        }
        /// <summary>
        /// Delete a review for a product.
        /// </summary>
        /// <param name="reviewId"></param>
        [Authorize]
        [HttpDelete("delete-product-review/{reviewId}")]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            var customerCode = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(customerCode))
            {
                return Unauthorized(new APIResponse(401, "Please Login or Register"));
            }

            var result = await _productReview.DeleteReviewAsync(reviewId, customerCode);
            return result ? Ok(new APIResponse(200, "Review Deleted Successfully"))
                          : NotFound(new APIResponse(404, "Review Not Found or Unauthorized"));
        }
    }
}
