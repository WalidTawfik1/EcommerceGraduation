using EcommerceGraduation.Core.DTO;
using EcommerceGraduation.Core.Entities;
using EcommerceGraduation.Core.Services;
using EcommerceGraduation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

                    DateOnly today = DateOnly.FromDateTime(DateTime.Now);
                    var UpdateShippingStatus = await dbContext.Shippings
                        .Where(o => o.ShippingStatus == Status.Pending && o.EstimatedDeliveryDate.Value == today)
                        .Include(o => o.OrderNumberNavigation)
                        .ToListAsync();

                    if (UpdateShippingStatus.Any())
                    {
                        foreach (var shipping in UpdateShippingStatus)
                        {
                            shipping.ShippingStatus = Status.Shipped;
                            shipping.OrderNumberNavigation.OrderStatus = Status.Delivered;
                            var userEmail = shipping.OrderNumberNavigation.CustomerCodeNavigation.Email;
                            var emailContent = $"Your order {shipping.OrderNumber} has been delivered.";
                            var emailDTO = new EmailDTO(
                                userEmail,
                                "maximlev643@gmail.com",
                                "Order Delivered",
                                emailContent
                            );
                            await emailService.SendEmailAsync(emailDTO);
                        }
                        await dbContext.SaveChangesAsync();
                    }
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }

}
