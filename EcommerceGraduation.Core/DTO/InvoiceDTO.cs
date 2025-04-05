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
        public IReadOnlyList<InvoiceDetailDTO> InvoiceDetails { get; set; }
    }
    public record InvoiceDetailDTO
    {
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal FinalPrice { get; set; }
        public decimal SubTotal { get; set; }
        public decimal DiscountPercent { get; set; }
    }
}
