using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceGraduation.Core.DTO
{
    public record InvoiceDTO
    {
        public string OrderNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal TotalBeforeDiscount { get; set; }
        public decimal ShippingValue { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalAfterDiscount { get; set; }
        public decimal Total { get; set; }
    }
}
