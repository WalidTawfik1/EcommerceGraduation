using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceGraduation.Core.Entities
{
    public class CartItem
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public String Category { get; set; }
        public String Photo { get; set; }
        public decimal TotalPrice => Price * Quantity;

    }
}
