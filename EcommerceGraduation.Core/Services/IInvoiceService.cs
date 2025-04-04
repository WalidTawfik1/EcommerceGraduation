using EcommerceGraduation.Core.DTO;
using EcommerceGraduation.Core.Entities;
using EcommerceGraduation.Core.Sharing;
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
        Task<IReadOnlyList<InvoiceDTO>> GetAllInvoicesForUserAsync(string CustomerCode);
        Task<InvoiceDTO> GetInvoiceByIdAsync(int invoiceId, string CustomerCode);
        Task<IReadOnlyList<InvoiceDTO>> GetAllInvoices(PageSkip page);
    }
}
