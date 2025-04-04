using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceGraduation.Core.Sharing
{
    public class ProductParams
    {
        public int pagenum { get; set; } = 1;
        public int Maxpagesize { get; set; } = 6;
        private int _pagesize = 3;

        public int pagesize
        {
            get { return _pagesize; }
            set { _pagesize = value > Maxpagesize ? Maxpagesize : value; }
        }

        public string? sort { get; set; }
        public string? categoryCode { get; set; }
        public string? search { get; set; }
        public string? subCategoryCode { get; set; }
        public string? brandCode { get; set; }
        public decimal? minPrice { get; set; }
        public decimal? maxPrice { get; set; }
    }
}