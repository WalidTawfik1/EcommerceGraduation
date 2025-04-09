using EcommerceGraduation.Core.Sharing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceGraduation.Core.DTO
{
    public record LoginDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public record RegisterDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public DateOnly DateOfBirth { get; set; }
    }

    public record ResetPasswordDTO: LoginDTO
    {
        public string Code { get; set; }
    }

    public record ActiveAccountDTO
    {
        public string Email { get; set; }
        public string Code { get; set; }
    }

}
