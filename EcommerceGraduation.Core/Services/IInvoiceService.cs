using EcommerceGraduation.Core.DTO;
using EcommerceGraduation.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceGraduation.Core.Services
{
    public interface IInvoiceService
    {
        Task <Invoice> GenerateInvoiceAsync(string orderNumber);
        Task<IReadOnlyList<InvoiceDTO>> GetAllOrdersForUserAsync(string CustomerCode);
        Task<InvoiceDTO> GetOrderByIdAsync(int invoiceId, string CustomerCode);
    }
}
