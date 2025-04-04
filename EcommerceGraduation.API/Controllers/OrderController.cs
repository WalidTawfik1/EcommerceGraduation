using EcommerceGraduation.Core.DTO;
using EcommerceGraduation.Core.Services;
using EcommerceGraduation.Core.Sharing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace EcommerceGraduation.API.Controllers
{
    /// <summary>
    /// Controller for managing orders.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Creates a new order, ShippingMethod should be [Standard, Express, سريع ,عادي].
        /// </summary>
        /// <param name="orderDTO">The order details.</param>
        /// <returns>The created order.</returns>
        [Authorize]
        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrderAsync([FromBody] OrderDTO orderDTO)
        {
            var customerClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (customerClaim == null)
            {
                return Unauthorized(new { message = "User is not authenticated" });
            }
            var customerCode = customerClaim.Value;
            try
            {
                var order = await _orderService.CreateOrderAsync(orderDTO, customerCode);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, innerException = ex.InnerException?.Message });
            }
        }

        /// <summary>
        /// Gets all orders for the authenticated user.
        /// </summary>
        /// <returns>A list of orders for the user.</returns>
        [Authorize]
        [HttpGet("get-all-orders-for-user")]
        public async Task<IActionResult> GetAllOrdersForUserAsync()
        {
            var customerClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (customerClaim == null)
            {
                return Unauthorized(new { message = "User is not authenticated" });
            }
            var customerCode = customerClaim.Value;
            try
            {
                var orders = await _orderService.GetAllOrdersForUserAsync(customerCode);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, innerException = ex.InnerException?.Message });
            }
        }

        /// <summary>
        /// Gets an order by its number.
        /// </summary>
        /// <param name="orderNumber">The order number.</param>
        /// <returns>The order with the specified number.</returns>
        [Authorize]
        [HttpGet("get-order-by-id-for-user")]
        public async Task<IActionResult> GetOrderByIdAsync(string orderNumber)
        {
            var customerClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (customerClaim == null)
            {
                return Unauthorized(new { message = "User is not authenticated" });
            }
            var customerCode = customerClaim.Value;
            try
            {
                var order = await _orderService.GetOrderByIdAsync(orderNumber, customerCode);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, innerException = ex.InnerException?.Message });
            }
        }
        /// <summary>
        /// Gets all orders for admin.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("get-all-orders")]
        public async Task<IActionResult> GetAllOrders([FromQuery] PageSkip page)
        {
            try
            {
                var orders = await _orderService.GetAllOrders(page);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, innerException = ex.InnerException?.Message });
            }
        }
    }
}
