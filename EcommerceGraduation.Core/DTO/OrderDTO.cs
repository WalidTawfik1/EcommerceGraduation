using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceGraduation.Core.DTO
{
    public record OrderDTO
    {
        public string CartId { get; set; }
        public ShippingAdressDTO ShippingDTO { get; set; }
    }

    public record ShippingAdressDTO
    {
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string ShippingMethod { get; set; }
        public bool UseProfileAddress { get; set; } = false;
        public bool UpdateProfileAddress { get; set; } = false;

    }
}
