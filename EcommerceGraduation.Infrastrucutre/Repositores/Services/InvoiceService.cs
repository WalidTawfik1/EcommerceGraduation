using AutoMapper;
using EcommerceGraduation.Core.DTO;
using EcommerceGraduation.Core.Entities;
using EcommerceGraduation.Core.Interfaces;
using EcommerceGraduation.Core.Services;
using EcommerceGraduation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceGraduation.Infrastrucutre.Repositores.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly EcommerceDbContext _context;
        private readonly IMapper _mapper;
        public InvoiceService(EcommerceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Invoice> GenerateInvoiceAsync(string orderNumber)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderNumber == orderNumber);
            if (order == null)
            {
                throw new Exception("Order not found");
            }

            var invoice = new Invoice
            {
                OrderNumber = orderNumber,
                InvoiceDate = DateTime.Now,
                TotalBeforeDiscount = order.TotalAmount - order.Shippings.Sum(s => s.ShippingCost),
                CustomerCode = order.CustomerCode,
                ShippingValue = order.Shippings.Sum(s => s.ShippingCost),
                DiscountPercent = order.OrderDetails.Sum(s => s.DiscountPercent),
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();
            return invoice;
        }

        public async Task<IReadOnlyList<InvoiceDTO>> GetAllOrdersForUserAsync(string CustomerCode)
        {
            var invoices = await _context.Invoices.Where(i => i.CustomerCode == CustomerCode).ToListAsync();
            var result = _mapper.Map<IReadOnlyList<InvoiceDTO>>(invoices);
            return result;
        }

        public async Task<InvoiceDTO> GetOrderByIdAsync(int invoiceId, string CustomerCode)
        {
            var invoice = await _context.Invoices.Where(i => i.InvoiceId == invoiceId && i.CustomerCode == CustomerCode).FirstOrDefaultAsync();
            if (invoice == null)
            {
                throw new Exception("Invoice not found");
            }
            var result = _mapper.Map<InvoiceDTO>(invoice);
            return result;

        }
    }
}
