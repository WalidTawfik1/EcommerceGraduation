using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceGraduation.Core.Entities
{
    public class Wishlist_Items
    {
        public int ProductId { get; set; }
        public String Name { get; set; }
        public decimal Price { get; set; }
        public String Photo { get; set; }
    }
    public class AddtoWhishlist
    {
        public int ProductId { get; set; }
    }
}
