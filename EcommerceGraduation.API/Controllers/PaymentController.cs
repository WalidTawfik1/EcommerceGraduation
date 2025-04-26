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
            IOrderService orderService
,
            IConfiguration configuration)
        {
            _paymobService = paymobService;
            _orderService = orderService;
            this.configuration = configuration;
        }

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
                    await _paymobService.UpdateOrderSuccess(orderId);
                    return Content(HtmlGenerator.GenerateSuccessHtml(), "text/html");
                }

                await _paymobService.UpdateOrderFailed(orderId);
                return Content(HtmlGenerator.GenerateFailedHtml(), "text/html");
            }

            return Content(HtmlGenerator.GenerateSecurityHtml(), "text/html");
        }

        /* [HttpPost("webhook/success")]
         public async Task<IActionResult> PaymentSuccess([FromForm] string transaction_id)
         {
             if (string.IsNullOrEmpty(transaction_id))
                 return BadRequest(new APIResponse(400, "Transaction ID is required"));

             try
             {
                 var order = await _paymobService.UpdateOrderSuccess(transaction_id);
                 return Ok(new APIResponse(200, "Payment successful", order));
             }
             catch (Exception ex)
             {
                 return StatusCode(500, new APIResponse(500, $"Payment success callback error: {ex.Message}"));
             }
         }*/

        /*[HttpPost("webhook/failure")]
        public async Task<IActionResult> PaymentFailure([FromForm] string transaction_id)
        {
            if (string.IsNullOrEmpty(transaction_id))
                return BadRequest(new APIResponse(400, "Transaction ID is required"));

            try
            {
                var order = await _paymobService.UpdateOrderFailed(transaction_id);
                return Ok(new APIResponse(200, "Payment failed", order));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new APIResponse(500, $"Payment failure callback error: {ex.Message}"));
            }
        }*/

        /*[HttpPost("webhook/callback")]
        [HttpGet("webhook/callback")]
        public async Task<IActionResult> PaymentCallback(CashInCallbackTransaction callback)
        {
            if (callback == null)
                return BadRequest(new APIResponse(400, "Callback data is required"));

            try
            {
                var order = await _paymobService.ProcessTransactionCallback(callback);
                return Ok(new APIResponse(200, "Payment callback processed successfully", order));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new APIResponse(500, $"Payment callback processing error: {ex.Message}"));
            }
        }*/



        /* [HttpGet("cashin-callback")]
         public ActionResult CashInCallback(
              [FromQuery] string hmac,
              [FromBody] CashInCallback callback,
              [FromServices] IPaymobCashInBroker broker
          )
         {
             if (callback.Type is null || callback.Obj is null)
             {
                 throw new InvalidOperationException("Unexpected transaction callback.");
             }

             var content = ((JsonElement)callback.Obj).GetRawText();

             switch (callback.Type.ToUpperInvariant())
             {
                 case CashInCallbackTypes.Transaction:
                     {
                         var transaction = JsonSerializer.Deserialize<CashInCallbackTransaction>(content, SerializerOptions)!;
                         var valid = broker.Validate(transaction, hmac);

                         if (!valid)
                         {
                             return BadRequest();
                         }

                         // TODO: Handle transaction.
                         var order = _paymobService.ProcessTransactionCallback(transaction);
                         return Ok(new APIResponse(200, "Transaction processed successfully.", order));
                     }
                 case CashInCallbackTypes.Token:
                     {
                         var token = JsonSerializer.Deserialize<CashInCallbackToken>(content, SerializerOptions)!;
                         var valid = broker.Validate(token, hmac);

                         if (!valid)
                         {
                             return BadRequest();
                         }

                         // TODO: Handle token.
                         return Ok("Token processed successfully.");
                     }
                 default:
                     throw new InvalidOperationException($"Unexpected {nameof(CashInCallbackTypes)} = {callback.Type}");
             }
         }*/

        /*[HttpGet("format-current-url")]
        public IActionResult FormatCurrentUrl()
        {
            try
            {
                // Get the base URL (scheme, host, path)
                var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}";

                // Get query string without the leading '?'
                var queryString = Request.QueryString.Value?.TrimStart('?');

                if (string.IsNullOrEmpty(queryString))
                {
                    return Ok(new
                    {
                        FormattedUrl = baseUrl
                    });
                }

                // Split the query string by '&' to get individual parameters
                var parameters = queryString.Split('&');

                // Join the parameters with '&\n' to add a new line after each parameter
                var formattedQueryString = string.Join("&\n", parameters);

                // Construct the formatted URL with a '?' followed by the formatted query string
                var formattedUrl = $"{baseUrl}?{formattedQueryString}";

                // Return the URL as text/plain to preserve formatting
                return Content(formattedUrl, "text/plain");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = $"An error occurred: {ex.Message}" });
            }
        }*/

        /* [HttpGet("format-current-url")]               
         public IActionResult FormatCurrentUrl()
         {
             try
             {
                 // Get the base URL (scheme, host, path)
                 var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}";

                 // Get query string without the leading '?'
                 var queryString = Request.QueryString.Value?.TrimStart('?');

                 if (string.IsNullOrEmpty(queryString))
                 {
                     return Ok(new
                     {
                         FormattedUrl = baseUrl
                     });
                 }

                 // Split the query string by '&' to get individual parameters
                 var parameters = queryString.Split('&');

                 // Join the parameters with '&\n' to add a new line after each parameter
                 var formattedQueryString = string.Join("&\n", parameters);

                 // Construct the formatted URL with a '?' followed by the formatted query string
                 var formattedUrl = $"{baseUrl}?{formattedQueryString}";

                 // Return the formatted URL in JSON format with explicit \n character for newlines
                 return Ok(new
                 {
                     FormattedUrl = formattedUrl
                 });
             }
             catch (Exception ex)
             {
                 return StatusCode(500, new { Error = $"An error occurred: {ex.Message}" });
             }
         }*/



    }




}
