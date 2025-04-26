using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceGraduation.Core.DTO
{
    public record ReturnOrderDTO
    {
        public string OrderNumber { get; set; }
        public string OrderDate { get; set; }
        public string OrderStatus { get; set; }
        public string PaymentStatus { get; set; }
        public decimal TotalAmount { get; set; }
        public IReadOnlyList<ReturnOrderDetailDTO> orderDetails { get; set; }
        public IReadOnlyList<ReturnOrderShippingDTO> shipping { get; set; }


    }

    public record ReturnOrderStatusDTO
    {
        public string OrderNumber { get; set; }
        public string OrderDate { get; set; }
        public string OrderStatus { get; set; }
        public string PaymentStatus { get; set; }
    }

    public record ReturnOrderDetailDTO
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string MainImage { get; set; }
        public int Quantity { get; set; }
        public decimal FinalPrice { get; set; }
        public decimal SubTotal { get; set; }
    }

    public record ReturnOrderShippingDTO
    {
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string ShippingMethod { get; set; }
        public string Carrier { get; set; }
        public string TrackingNumber { get; set; }
        public string ShippingStatus { get; set; }
    }
}
