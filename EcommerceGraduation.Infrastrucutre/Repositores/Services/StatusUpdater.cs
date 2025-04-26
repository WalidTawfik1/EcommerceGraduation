using EcommerceGraduation.Core.DTO;
using EcommerceGraduation.Core.Entities;
using EcommerceGraduation.Core.Services;
using EcommerceGraduation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceGraduation.Infrastrucutre.Repositores.Services
{
    public class StatusUpdater : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<StatusUpdater> _logger;

        public StatusUpdater(IServiceScopeFactory scopeFactory, ILogger<StatusUpdater> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                // Run immediately on startup
                await UpdateStatuses(stoppingToken);

                // Then schedule to run daily
                using var timer = new PeriodicTimer(TimeSpan.FromHours(24));

                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    await UpdateStatuses(stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                // Normal shutdown, no need to log
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the StatusUpdater service");
            }
        }

        private async Task UpdateStatuses(CancellationToken stoppingToken)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<EcommerceDbContext>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                DateOnly today = DateOnly.FromDateTime(DateTime.Now);

                var shippingsToUpdate = await dbContext.Shippings
                    .Where(o => o.ShippingStatus == Status.Pending &&
                               o.EstimatedDeliveryDate.HasValue &&
                               o.EstimatedDeliveryDate.Value == today &&
                               o.OrderNumber != null &&
                               o.OrderNumberNavigation != null &&
                               o.OrderNumberNavigation.OrderStatus != Status.PaymentFailed)
                    .Include(o => o.OrderNumberNavigation)
                    .ThenInclude(o => o.CustomerCodeNavigation)
                    .ToListAsync(stoppingToken);

                foreach (var shipping in shippingsToUpdate)
                {
                    try
                    {
                        // Skip if OrderNumberNavigation is null
                        if (shipping.OrderNumberNavigation == null)
                        {
                            _logger.LogWarning("Order navigation is null for shipping record {ShippingId}", shipping.ShippingId);
                            continue;
                        }

                        shipping.ShippingStatus = Status.Delivered;
                        shipping.OrderNumberNavigation.OrderStatus = Status.Delivered;

                        // Skip if CustomerCodeNavigation is null
                        if (shipping.OrderNumberNavigation.CustomerCodeNavigation == null)
                        {
                            _logger.LogWarning("Customer navigation is null for order {OrderNumber}", shipping.OrderNumber);
                            continue;
                        }

                        var userEmail = shipping.OrderNumberNavigation.CustomerCodeNavigation.Email;

                        if (!string.IsNullOrEmpty(userEmail))
                        {
                            var emailContent = $@"
    <html>
        <body style='font-family: Arial, sans-serif; color: #333;'>
            <h2>Order Delivered</h2>
            <p>Dear Customer,</p>
            <p>We’re happy to inform you that your order <strong>{shipping.OrderNumber}</strong> has been successfully delivered.</p>

            <p>We hope you’re enjoying your purchase! If you have any questions or feedback, we’d love to hear from you.</p>

            <p>Thank you for shopping with us!</p>
            <p>Best regards,<br/>The SMarket Team</p>
        </body>
    </html>";

                            var emailDTO = new EmailDTO(
                                userEmail,
                                "smarket.ebusiness@gmail.com",
                                "Order Delivered",
                                emailContent
                            );

                            await emailService.SendEmailAsync(emailDTO);
                            _logger.LogInformation("Email sent to {Email} for order {OrderNumber}", userEmail, shipping.OrderNumber);
                        }
                        else
                        {
                            _logger.LogWarning("Customer email is empty for order {OrderNumber}", shipping.OrderNumber);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing shipping update for order {OrderNumber}", shipping.OrderNumber ?? "(null)");
                        // Continue with other orders even if one fails
                    }
                }

                if (shippingsToUpdate.Any())
                {
                    await dbContext.SaveChangesAsync(stoppingToken);
                    _logger.LogInformation("Updated status for {Count} orders", shippingsToUpdate.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateStatuses");
                throw;
            }
        }
    }

}
