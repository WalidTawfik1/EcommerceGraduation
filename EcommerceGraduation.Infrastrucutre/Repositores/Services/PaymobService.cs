using EcommerceGraduation.Core.DTO;
using EcommerceGraduation.Core.Entities;
using EcommerceGraduation.Core.Interfaces;
using EcommerceGraduation.Core.Services;
using EcommerceGraduation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using X.Paymob.CashIn;
using X.Paymob.CashIn.Models.Callback;
using X.Paymob.CashIn.Models.Orders;
using X.Paymob.CashIn.Models.Payment;
using X.Paymob.CashIn.Models.Transactions;

namespace EcommerceGraduation.Infrastrucutre.Repositores.Services
{
    public class PaymobService : IPaymobService
    {
        private readonly IUnitofWork _unitofWork;
        private readonly IConfiguration _configuration;
        private readonly EcommerceDbContext _context;
        private readonly IPaymobCashInBroker _broker;
        private readonly IEmailService _emailService;

        public PaymobService(
            EcommerceDbContext context,
            IConfiguration configuration,
            IUnitofWork unitofWork,
            IPaymobCashInBroker broker,
            IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _unitofWork = unitofWork;
            _broker = broker;
            _emailService = emailService;
        }

        public async Task<Cart> CreateOrUpdatePaymentAsync(string cartId)
        {
            // Get cart from repository
            var cart = await _unitofWork.CartRepository.GetCartAsync(cartId);

            if (cart == null || !cart.Items.Any())
            {
                throw new InvalidOperationException("Cart is empty or does not exist.");
            }

            // Update cart item prices from current product prices
            foreach (var item in cart.Items)
            {
                var product = await _unitofWork.ProductRepository.GetByIdAsync(item.ProductId);
                if (product != null)
                {
                    item.Price = product.Price;
                }
            }

            // Convert amount to cents as required by Paymob (100 piasters = 1 EGP)
            var amountCents = (int)(cart.SubAmount * 100);

            // Create Paymob order
            var orderRequest = CashInCreateOrderRequest.CreateOrder(amountCents);
            var orderResponse = await _broker.CreateOrderAsync(orderRequest);

            // Add payment intent ID to the cart for tracking
            cart.PaymentIntentId = orderResponse.Id.ToString();

            // Update cart in repository
            await _unitofWork.CartRepository.UpdateCartAsync(cart);

            return cart;
        }

        public async Task<Order> ProcessPaymentForOrderAsync(string orderNumber)
        {
            // Find the order
            var order = await _context.Orders
                .Include(o => o.Shippings)
                .Include(o => o.CustomerCodeNavigation)
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

            if (order == null)
            {
                throw new InvalidOperationException($"Order with number {orderNumber} not found.");
            }

            // Get the shipping information (for billing data)
            var shipping = order.Shippings.FirstOrDefault();
            if (shipping == null)
            {
                throw new InvalidOperationException($"No shipping information found for order {orderNumber}.");
            }

            // Get customer information
            var customer = order.CustomerCodeNavigation;
            if (customer == null)
            {
                throw new InvalidOperationException($"No customer found for order {orderNumber}.");
            }

            // Prepare amount in cents
            var amountCents = (int)(order.TotalAmount * 100 ?? 0);

            // Create Paymob order
            var orderRequest = CashInCreateOrderRequest.CreateOrder(amountCents);
            var orderResponse = await _broker.CreateOrderAsync(orderRequest);

            // Get customer name parts (first name, last name)
            string firstName = customer.Name?.Split(' ').FirstOrDefault() ?? "Guest";
            string lastName = customer.Name?.Split(' ').Skip(1).FirstOrDefault() ?? "User";

            // Create billing data from shipping information
            var billingData = new CashInBillingData(
                firstName: firstName,
                lastName: lastName,
                phoneNumber: customer.PhoneNumber,
                email: customer.Email,
                country: shipping.Country,
                state:"N/A",
                city: shipping.City,
                apartment: "N/A",
                street: shipping.Address,
                floor: "N/A",
                building: "N/A",
                shippingMethod: shipping.ShippingMethod.ToString(),
                postalCode: shipping.PostalCode);           

            // Get integration ID from configuration
            if (!int.TryParse(_configuration["Paymob:IntegrationId"], out int integrationId))
            {
                throw new InvalidOperationException("Invalid Paymob integration ID in configuration.");
            }

            // Create payment key request
            var paymentKeyRequest = new CashInPaymentKeyRequest
            (
                integrationId: integrationId,
                orderId: orderResponse.Id,
                billingData: billingData,
                amountCents: amountCents,
                currency: "EGP",
                lockOrderWhenPaid: true,
                expiration: null

            );

            // Request payment key from Paymob
            var paymentKeyResponse = await _broker.RequestPaymentKeyAsync(paymentKeyRequest);

            // Create a new payment record
            var payment = new Payment
            {
                OrderNumber = order.OrderNumber,
                TransactionId = orderResponse.Id.ToString(),
                Amount = (decimal)order.TotalAmount,
                PaymentDate = DateTime.Now,
                PaymentMethod = "Paymob",
                PaymentStatus = "Pending"
            };

            // Add payment record to database
            _context.Payments.Add(payment);

            // Update order status
            order.PaymentStatus = Status.Pending;

            // Save changes
            await _context.SaveChangesAsync();

            // Store payment token for iframe URL
            order.PaymentToken = paymentKeyResponse.PaymentKey;

            // Save order changes
            await _context.SaveChangesAsync();

            return order;
        }

        public string GetPaymentIframeUrl(string paymentToken)
        {
            if (string.IsNullOrEmpty(paymentToken))
            {
                throw new ArgumentNullException(nameof(paymentToken), "Payment token cannot be null or empty.");
            }

            string iframeId = _configuration["Paymob:IframeId"];
            if (string.IsNullOrEmpty(iframeId))
            {
                throw new InvalidOperationException("Paymob iframe ID is not configured.");
            }

            // Build the Paymob iframe URL
            string iframeUrl = $"https://accept.paymob.com/api/acceptance/iframes/{iframeId}?payment_token={paymentToken}";
            return iframeUrl;
        }

        public async Task<Order> UpdateOrderSuccess(string paymentIntentId)
        {
            // Find the payment record by transaction ID
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.TransactionId == paymentIntentId);

            if (payment == null)
            {
                throw new InvalidOperationException($"Payment with transaction ID {paymentIntentId} not found.");
            }

            // Find the order linked to this payment
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .Include(o => o.Shippings)
                .Include(o => o.CustomerCodeNavigation)
                .FirstOrDefaultAsync(o => o.OrderNumber == payment.OrderNumber);

            var customerCode = order.CustomerCode;

            if (order == null)
            {
                throw new InvalidOperationException($"Order with number {payment.OrderNumber} not found.");
            }

            // Update order status
            order.OrderStatus = Status.Shipped;
            order.PaymentStatus = Status.Success;

            // Update shipping status if needed
            var shipping = await _context.Shippings.FirstOrDefaultAsync(o => o.OrderNumber == order.OrderNumber);
            if (shipping != null)
            {
                shipping.ShippingStatus = Status.Shipped;
            }

            // Update payment record
            payment.PaymentStatus = "Completed";
            payment.PaymentDate = DateTime.Now;

            // Save changes
            await _unitofWork.CartRepository.DeleteCartAsync(customerCode);
            _context.Orders.Update(order);
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();

            await SendOrderSuccessEmailAsync(order, shipping);

            return order;
        }

        public async Task<Order> UpdateOrderFailed(string paymentIntentId)
        {
            // Find the payment record by transaction ID
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.TransactionId == paymentIntentId);

            if (payment == null)
            {
                throw new InvalidOperationException($"Payment with transaction ID {paymentIntentId} not found.");
            }

            // Find the order linked to this payment
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderNumber == payment.OrderNumber);

            if (order == null)
            {
                throw new InvalidOperationException($"Order with number {payment.OrderNumber} not found.");
            }

            // Update order status
            order.OrderStatus = Status.PaymentFailed;
            order.PaymentStatus = Status.Failed;

            // Update shipping status if needed
            var shipping = await _context.Shippings.FirstOrDefaultAsync(o => o.OrderNumber == order.OrderNumber);
            if (shipping != null)
            {
                shipping.ShippingStatus = Status.PaymentFailed;
            }

            // Update payment record
            payment.PaymentStatus = "Failed";
            payment.PaymentDate = DateTime.Now;

            // Save changes
            _context.Orders.Update(order);
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();

            return order;
        }

        private async Task SendOrderSuccessEmailAsync(Order order, Shipping shipping)
        {
            var customerEmail = order.CustomerCodeNavigation?.Email;
            if (!string.IsNullOrEmpty(customerEmail))
            {
                var emailContent = $@"
    <html>
        <body style='font-family: Arial, sans-serif; color: #333;'>
            <h2>Payment Successful!</h2>
            <p>Dear {order.CustomerCodeNavigation.Name},</p>
            <p>Great news! Your payment for order #{order.OrderNumber} has been successfully processed. We're preparing your items for shipping.</p>

            <table style='border-collapse: collapse; width: 100%; max-width: 600px;'>
                <tr>
                    <td style='padding: 8px; font-weight: bold;'>Order Number:</td>
                    <td style='padding: 8px;'>{order.OrderNumber}</td>
                </tr>
                <tr>
                    <td style='padding: 8px; font-weight: bold;'>Order Date:</td>
                    <td style='padding: 8px;'>{order.OrderDate:MMMM dd, yyyy}</td>
                </tr>
                <tr>
                    <td style='padding: 8px; font-weight: bold;'>Payment Status:</td>
                    <td style='padding: 8px; color: green;'>Successful</td>
                </tr>
                <tr>
                    <td style='padding: 8px; font-weight: bold;'>Shipping Method:</td>
                    <td style='padding: 8px;'>{shipping?.ShippingMethod}</td>
                </tr>
                <tr>
                    <td style='padding: 8px; font-weight: bold;'>Estimated Delivery Date:</td>
                    <td style='padding: 8px;'>{shipping?.EstimatedDeliveryDate:MMMM dd, yyyy}</td>
                </tr>
                <tr>
                    <td style='padding: 8px; font-weight: bold;'>Tracking Number:</td>
                    <td style='padding: 8px;'>{shipping?.TrackingNumber}</td>
                </tr>
                <tr>
                    <td style='padding: 8px; font-weight: bold;'>Total Amount:</td>
                    <td style='padding: 8px;'>{order.TotalAmount:F2}</td>
                </tr>
            </table>

            <p>You'll receive another notification when your order ships. If you have any questions or need further assistance, feel free to contact our support team.</p>
            <p>Thank you for shopping with us!</p>
            <p>Best regards,<br/>The SMarket Team</p>
        </body>
    </html>";

                var emailDTO = new EmailDTO(
                    customerEmail,
                    "smarket.ebusiness@gmail.com",
                    "Payment Successful - Order #" + order.OrderNumber,
                    emailContent
                );

                await _emailService.SendEmailAsync(emailDTO);
            }
        }

        public async Task<Order> ProcessTransactionCallback(CashInCallbackTransaction callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback), "Callback data is null");
            }

            // Log callback data for debugging - fixing the incorrect string interpolation
            Console.WriteLine($"Received transaction callback for ID: {callback.Id}");

            // The CashInCallbackTransaction structure has changed - it no longer has a Transaction property
            // Instead, we need to use the properties directly from the callback object

            // Find the payment record by transaction ID from Paymob
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.TransactionId == callback.Id.ToString());

            if (payment == null)
            {
                // Try to find by order ID if available
                var orderId = callback.Order.Id.ToString();
                payment = await _context.Payments
                    .FirstOrDefaultAsync(p => p.TransactionId == orderId);

                if (payment == null)
                {
                    // If still not found, create a new payment record if we can find the order
                    var order = await _context.Orders
                        .FirstOrDefaultAsync(o => o.OrderNumber == callback.Order.Id.ToString());

                    if (order == null)
                    {
                        throw new InvalidOperationException($"No order found with ID {callback.Order.Id}");
                    }

                    payment = new Payment
                    {
                        OrderNumber = order.OrderNumber,
                        TransactionId = callback.Id.ToString(),
                        Amount = (decimal)(callback.AmountCents / 100.0), // Convert back from cents
                        PaymentDate = DateTime.Now,
                        PaymentMethod = "Paymob",
                        PaymentStatus = "Pending"
                    };

                    _context.Payments.Add(payment);
                    await _context.SaveChangesAsync();
                }
            }

            // Find the order linked to this payment
            var orderToUpdate = await _context.Orders
                .Include(o => o.OrderDetails)
                .Include(o => o.Shippings)
                .FirstOrDefaultAsync(o => o.OrderNumber == payment.OrderNumber);

            if (orderToUpdate == null)
            {
                throw new InvalidOperationException($"Order with number {payment.OrderNumber} not found");
            }

            // Update order and payment status based on the transaction callback
            switch (callback.ApiSource)
            {
                case "IFRAME":
                case "INVOICE":
                    // Process transaction status
                    if (callback.Success && !callback.IsVoided && !callback.IsRefunded)
                    {
                        // Payment successful
                        orderToUpdate.OrderStatus = Status.Paid;
                        orderToUpdate.PaymentStatus = Status.PaymentReceived;
                        payment.PaymentStatus = "Successful";

                        // Update shipping status if needed
                        foreach (var shipping in orderToUpdate.Shippings)
                        {
                            shipping.ShippingStatus = Status.Pending;
                        }
                    }
                    else if (callback.IsRefunded)
                    {
                        // Payment refunded
                        orderToUpdate.OrderStatus = Status.Pending;
                        orderToUpdate.PaymentStatus = Status.PaymentFailed;
                        payment.PaymentStatus = "Refunded";
                    }
                    else if (callback.IsVoided)
                    {
                        // Payment voided
                        orderToUpdate.OrderStatus = Status.Pending;
                        orderToUpdate.PaymentStatus = Status.PaymentFailed;
                        payment.PaymentStatus = "Voided";
                    }
                    else
                    {
                        // Payment failed
                        orderToUpdate.OrderStatus = Status.Pending;
                        orderToUpdate.PaymentStatus = Status.PaymentFailed;
                        payment.PaymentStatus = "Failed";
                    }
                    break;

                default:
                    // Unknown callback type, log but don't change status
                    Console.WriteLine($"Unhandled API source: {callback.ApiSource}");
                    break;
            }

            payment.PaymentDate = DateTime.Now;

            // Save changes
            _context.Orders.Update(orderToUpdate);
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();

            return orderToUpdate;
        }

        public string ComputeHmacSHA512(string data, string secret)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secret);
            var dataBytes = Encoding.UTF8.GetBytes(data);

            using (var hmac = new HMACSHA512(keyBytes))
            {
                var hash = hmac.ComputeHash(dataBytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}
