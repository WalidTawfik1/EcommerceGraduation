using EcommerceGraduation.Core.Interfaces;
using EcommerceGraduation.Core.Services;
using EcommerceGraduation.Infrastructure.Data;
using EcommerceGraduation.Infrastrucutre.Repositores;
using EcommerceGraduation.Infrastrucutre.Repositores.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceGraduation.Infrastrucutre
{
    public static class InfrastructureRegistration
    {
        public static IServiceCollection InfrastructureConfiguration(this IServiceCollection services, IConfiguration configuration)
        {


            // Register the IUnitofWork service
            services.AddScoped<IUnitofWork, UnitofWork>();

            // Register the IImageManagmentService service
            services.AddSingleton<IProductImageManagmentService, ProductImageManagementService>();
            // Register the IFileProvider service
            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));

            // Register the AppDbContext with SQL Server
            services.AddDbContext<EcommerceDbContext>(option =>
            {
                option.UseSqlServer(configuration.GetConnectionString("EcommerceDatabase"));
            });

            services.AddSingleton<IConnectionMultiplexer>(i =>
            {

                var config = ConfigurationOptions.Parse(configuration.GetConnectionString("Redis"));
                return ConnectionMultiplexer.Connect(config);
            });

            return services;
        }
    }
}
