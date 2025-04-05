using EcommerceGraduation.Core.DTO;
using EcommerceGraduation.Core.Entities;
using EcommerceGraduation.Core.Services;
using EcommerceGraduation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EcommerceGraduation.Infrastrucutre.Repositores.Services
{
    public class StatusUpdater : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public StatusUpdater(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<EcommerceDbContext>();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                    try
                    {
                        DateOnly today = DateOnly.FromDateTime(DateTime.Now);

                        var updateShippingStatus = await dbContext.Shippings
                            .Where(o => o.ShippingStatus == Status.Pending && o.EstimatedDeliveryDate.Value == today)
                            .Include(o => o.OrderNumberNavigation)
                                .ThenInclude(o => o.CustomerCodeNavigation)
                            .ToListAsync(stoppingToken);

                        if (updateShippingStatus.Any())
                        {
                            foreach (var shipping in updateShippingStatus)
                            {
                                if (shipping.OrderNumberNavigation == null ||
                                    shipping.OrderNumberNavigation.CustomerCodeNavigation == null)
                                {
                                    // Optional debug logging
                                    Console.WriteLine($"[Warning] Missing order or customer info for ShippingID: {shipping.ShippingId}");
                                    continue;
                                }

                                shipping.ShippingStatus = Status.Shipped;
                                shipping.OrderNumberNavigation.OrderStatus = Status.Delivered;

                                var userEmail = shipping.OrderNumberNavigation.CustomerCodeNavigation.Email;
                                if (!string.IsNullOrEmpty(userEmail))
                                {
                                    var emailContent = $"Your order {shipping.OrderNumber} has been delivered.";
                                    var emailDTO = new EmailDTO(
                                        userEmail,
                                        "maximlev643@gmail.com",
                                        "Order Delivered",
                                        emailContent
                                    );
                                    await emailService.SendEmailAsync(emailDTO);
                                }
                            }

                            await dbContext.SaveChangesAsync(stoppingToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log exception details to the console or a logger (optional)
                        Console.WriteLine($"[Error] StatusUpdater failed: {ex.Message}");
                    }
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }
}
