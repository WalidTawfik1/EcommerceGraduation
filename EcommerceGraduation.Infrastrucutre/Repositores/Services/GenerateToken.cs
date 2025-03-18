using EcommerceGraduation.Core.Entities;
using EcommerceGraduation.Core.Services;
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

        public GenerateToken(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetAndCreateToken(Customer customer)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, customer.Name),
                new Claim(ClaimTypes.Email, customer.Email),
                new Claim(ClaimTypes.Role, customer.UserType),
                new Claim(ClaimTypes.NameIdentifier, customer.Id)
            };

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
