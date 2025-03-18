using EcommerceGraduation.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceGraduation.Core.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailDTO emailDTO);
    }
}
