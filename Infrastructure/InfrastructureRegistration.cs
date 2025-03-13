using Ecom.Core.Interfaces;
using Ecom.Core.Services;
using Ecom.Infrastructure.Data;
using Ecom.Infrastructure.Repositores;
using Ecom.Infrastructure.Repositores.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.VisualBasic;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure
{
    public static class InfrastructureRegistration
    {
        public static IServiceCollection InfrastructureConfiguration(this IServiceCollection services, IConfiguration configuration)
        {


            // Register the IUnitofWork service
            services.AddScoped<IUnitofWork, UnitofWork>();

            // Register the IImageManagmentService service
            services.AddSingleton<IImageManagmentService, ImageMangementService>();
            // Register the IFileProvider service
            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));

            // Register the AppDbContext with SQL Server
            services.AddDbContext<AppDbContext>(option =>
            {
                option.UseSqlServer(configuration.GetConnectionString("EcomCS"));
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
