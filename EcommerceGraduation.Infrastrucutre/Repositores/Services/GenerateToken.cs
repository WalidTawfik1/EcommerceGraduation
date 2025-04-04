using EcommerceGraduation.Core.Entities;
using EcommerceGraduation.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceGraduation.Infrastrucutre.Repositores.Services
{
    public class GenerateToken : IGenerateToken
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<Customer> _userManager;

        public GenerateToken(IConfiguration configuration, UserManager<Customer> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public string GetAndCreateToken(Customer customer)
        {
            var role = _userManager.GetRolesAsync(customer).Result;
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, customer.Name),
                new Claim(ClaimTypes.Email, customer.Email),
                new Claim(ClaimTypes.NameIdentifier, customer.Id)
            };
            foreach (var item in role)
            {
                claims.Add(new Claim(ClaimTypes.Role, item));
            }

            var security = _configuration["Token:Secret"];
            var key = Encoding.ASCII.GetBytes(security);
            var SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(int.Parse(_configuration["Token:ExpiryDays"])),
                Issuer = _configuration["Token:Issuer"],
                SigningCredentials = SigningCredentials,
                NotBefore = DateTime.Now
            };
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(token);
        }
    }
}
