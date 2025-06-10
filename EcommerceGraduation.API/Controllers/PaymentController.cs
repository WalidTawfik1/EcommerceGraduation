using EcommerceGraduation.API.Helper;
using EcommerceGraduation.API.Helper.EcommerceGraduation.API.Helper;
using EcommerceGraduation.Core.DTO;
using EcommerceGraduation.Core.Entities;
using EcommerceGraduation.Core.Interfaces;
using EcommerceGraduation.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MsgPack.Serialization;
using System;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using X.Paymob.CashIn;
using X.Paymob.CashIn.Models.Callback;

namespace EcommerceGraduation.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymobService _paymobService;
        private readonly IOrderService _orderService;
        private readonly IConfiguration configuration;

        public PaymentController(
            IPaymobService paymobService,
            IOrderService orderService,
            IConfiguration configuration)
        {
            _paymobService = paymobService;
            _orderService = orderService;
            this.configuration = configuration;
        }

        /// <summary>
        /// Initiates payment processing for an order by its order number.
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("process-payment/{orderNumber}")]
        public async Task<IActionResult> ProcessPayment(string orderNumber)
        {
            if (string.IsNullOrEmpty(orderNumber))
                return BadRequest(new APIResponse(400, "Order number is required"));

            try
            {
                // Process payment for the order
                var order = await _paymobService.ProcessPaymentForOrderAsync(orderNumber);

                // Get payment iframe URL
                var iframeUrl = _paymobService.GetPaymentIframeUrl(order.PaymentToken);

                return Ok(new APIResponse(200, "Payment processing initiated", new
                {
                    OrderNumber = order.OrderNumber,
                    PaymentUrl = iframeUrl,
                    PaymentToken = order.PaymentToken
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new APIResponse(500, $"Payment processing error: {ex.Message}"));
            }
        }

        [Authorize]
        [HttpPost("create-order-with-payment")]
        public async Task<IActionResult> CreateOrderWithPayment([FromBody] OrderDTO orderDTO)
        {
            var customerClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (customerClaim == null)
            {
                return Unauthorized(new { message = "User is not authenticated" });
            }
            var customerCode = customerClaim.Value;

            try
            {
                // Step 1: Create the order
                var order = await _orderService.CreateOrderAsync(orderDTO, customerCode);

                // Step 2: Process payment
                order = await _paymobService.ProcessPaymentForOrderAsync(order.OrderNumber);

                // Step 3: Get payment iframe URL
                var iframeUrl = _paymobService.GetPaymentIframeUrl(order.PaymentToken);

                return Ok(new APIResponse(200, "Order created and payment initiated", new
                {
                    Order = order,
                    PaymentUrl = iframeUrl,
                    PaymentToken = order.PaymentToken
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new APIResponse(500, $"Error creating order with payment: {ex.Message}"));
            }
        }

        /// <summary>
        /// Handles the callback from Paymob after payment processing.
        /// </summary>
        /// <returns></returns>
        [HttpGet("callback")]
        public async Task<IActionResult> CallbackAsync()
        {
            var query = Request.Query;

            string[] fields = new[]
            {
                "amount_cents", "created_at", "currency", "error_occured", "has_parent_transaction",
                "id", "integration_id", "is_3d_secure", "is_auth", "is_capture", "is_refunded",
                "is_standalone_payment", "is_voided", "order", "owner", "pending",
                "source_data.pan", "source_data.sub_type", "source_data.type", "success"
            };

            var concatenated = new StringBuilder();
            foreach (var field in fields)
            {
                if (query.TryGetValue(field, out var value))
                {
                    concatenated.Append(value);
                }
                else
                {
                    return BadRequest($"Missing expected field: {field}");
                }
            }

            string receivedHmac = query["hmac"];
            string calculatedHmac = _paymobService.ComputeHmacSHA512(concatenated.ToString(), configuration["Paymob:HMAC"]);

            if (receivedHmac.Equals(calculatedHmac, StringComparison.OrdinalIgnoreCase))
            {
                bool.TryParse(query["success"], out bool isSuccess);
                string orderId = query["order"];

                if (isSuccess)
                {
                    try
                    {
                        await _paymobService.UpdateOrderSuccess(orderId);
                        return Content(HtmlGenerator.GenerateSuccessHtml(), "text/html");
                    }
                    catch (Exception ex)
                    {
                        // Log the exception if you have logging configured
                        Console.WriteLine($"Error updating order success: {ex.Message}");
                        return Content(HtmlGenerator.GenerateFailedHtml(), "text/html");
                    }
                }

                try
                {
                    await _paymobService.UpdateOrderFailed(orderId);
                    return Content(HtmlGenerator.GenerateFailedHtml(), "text/html");
                }
                catch (Exception ex)
                {
                    // Log the exception with inner exception details
                    Console.WriteLine($"Error updating order failure: {ex.Message}");
                    Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                    Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                    // Return detailed error information for debugging
                    var errorDetails = new
                    {
                        Message = ex.Message,
                        InnerExceptionMessage = ex.InnerException?.Message,
                        StackTrace = ex.StackTrace
                    };

                    // Return the detailed error information
                    return StatusCode(500, JsonSerializer.Serialize(errorDetails));
                }
            }

            return Content(HtmlGenerator.GenerateSecurityHtml(), "text/html");
        }
    }
}
