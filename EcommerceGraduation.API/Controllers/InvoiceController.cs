using EcommerceGraduation.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcommerceGraduation.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }
        /// <summary>
        /// Get all invoices for the authenticated user.
        /// </summary>
        /// <returns> A list of invoices for the authenticated user.</returns>
        [Authorize]
        [HttpGet("get-all-invoices-for-user")]
        public async Task<IActionResult> GetAllInvoicesForUserAsync()
        {
            var customerClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (customerClaim == null)
            {
                return Unauthorized(new { message = "User is not authenticated" });
            }
            var customerCode = customerClaim.Value;
            try
            {
                var invoices = await _invoiceService.GetAllOrdersForUserAsync(customerCode);
                return Ok(invoices);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, innerException = ex.InnerException?.Message });
            }
        }
        /// <summary>
        /// Get an invoice by its id.
        /// </summary>
        /// <param name="invoiceId">The ID of the invoice to retrieve.</param>
        /// <returns>The invoice with the specified ID.</returns>
        [Authorize]
        [HttpGet("get-invoice-by-id")]
        public async Task<IActionResult> GetInvoiceByIdAsync(int invoiceId)
        {
            var customerClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (customerClaim == null)
            {
                return Unauthorized(new { message = "User is not authenticated" });
            }
            var customerCode = customerClaim.Value;
            try
            {
                var invoice = await _invoiceService.GetOrderByIdAsync(invoiceId, customerCode);
                return Ok(invoice);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, innerException = ex.InnerException?.Message });
            }
        }


    }
}
