using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceGraduation.Core.Entities
{
    public class Wishlist
    {
        public Wishlist() { }
        public Wishlist(string id)
        {
            Id = id;
        }
        public string Id { get; set; }
        public List<Wishlist_Items> Items { get; set; } = new List<Wishlist_Items>();

    }
}
