using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceGraduation.Core.DTO
{
    public record OTPVerify
    {
        public string Email { get; set; }
        public string Code { get; set; }
    }

    public record OTPResend
    {
        public string Email { get; set; }
    }
}
