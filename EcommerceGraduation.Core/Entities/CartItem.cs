﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceGraduation.Core.Entities
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public String Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public String Photo { get; set; }
        public decimal TotalPrice => Price * Quantity;

    }

    public class AddtoCart
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }

    }
}
