using EcommerceGraduation.Core.Entities;
using EcommerceGraduation.Core.Interfaces;
using EcommerceGraduation.Core.Services;
using EcommerceGraduation.Infrastructure.Data;
using EcommerceGraduation.Infrastrucutre.Repositores;
using EcommerceGraduation.Infrastrucutre.Repositores.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceGraduation.Infrastrucutre
{
    public static class InfrastructureRegistration
    {
        public static object InfrastructureConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            // Register the IUnitofWork service
            services.AddScoped<IUnitofWork, UnitofWork>();

            // Register the IImageManagmentService service
            services.AddSingleton<IProductImageManagmentService, ProductImageManagementService>();
            // Register the IFileProvider service
            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));

            // Register the IEmailService service
            services.AddScoped<IEmailService, EmailService>();

            // Register the IGenerateToken service
            services.AddScoped<IGenerateToken, GenerateToken>();

            // Register the IOrderService service
            services.AddScoped<IOrderService, OrderService>();

            // Register the IInvoiceService service
            services.AddScoped<IInvoiceService, InvoiceService>();

            // Register the AppDbContext with SQL Server
            services.AddDbContext<EcommerceDbContext>((options) =>
            {
                options.UseSqlServer(configuration.GetConnectionString("EcommerceDatabase"));
            });

            services.AddSingleton<IConnectionMultiplexer>(i =>
            {
                var config = ConfigurationOptions.Parse(configuration.GetConnectionString("Redis"));
                return ConnectionMultiplexer.Connect(config);
            });

            services.AddIdentity<Customer, IdentityRole<string>>(opt =>
            {
                opt.Password.RequireDigit = true;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireUppercase = true;
                opt.Password.RequireNonAlphanumeric = true;
                opt.Password.RequiredLength = 6;
            }).AddEntityFrameworkStores<EcommerceDbContext>().AddDefaultTokenProviders();

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(c =>
            {
                c.Cookie.Name = "token";
                c.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };

            }).AddJwtBearer(opt =>
            {
                opt.RequireHttpsMetadata = false;
                opt.SaveToken = true;
                opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Token:Secret"])),
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Token:Issuer"],
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
                opt.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies["token"];
                        return Task.CompletedTask;
                    }
                };
            });

            // Allow user tracking
            services.AddHttpContextAccessor();

            return services;
        }
    }
}
