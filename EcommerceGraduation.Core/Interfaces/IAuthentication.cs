using EcommerceGraduation.Core.DTO;
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

    }
}
