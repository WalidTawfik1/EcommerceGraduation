using EcommerceGraduation.Core.DTO;
using EcommerceGraduation.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceGraduation.Core.Interfaces
{
    public interface IAuthentication
    {
        Task<string> RegisterAsync(RegisterDTO registerDTO);
        Task<string> LoginAsync(LoginDTO loginDTO);
        Task<bool> SendEmailForgetPassword(string email);
        Task<string> ResetPassword(ResetPasswordDTO resetPasswordDTO);
        Task<bool> ActiveAccount(ActiveAccountDTO activeAccountDTO);
        Task StoreOtpForUser(string email, string otpCode, string purpose = "VerifyEmail");
        Task SendEmailWithOtp(string email, string otpCode, string message);
        Task<bool> CheckOtpCode(string email, string otpCode);
        Task<bool> ResendOtpCode(string email);
    }
}
