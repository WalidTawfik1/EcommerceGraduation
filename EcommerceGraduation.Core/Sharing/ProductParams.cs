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
        public int Maxpagesize { get; set; } = 50;
        private int _pagesize;

        public int pagesize
        {
            get { return _pagesize; }
            set { _pagesize = value > Maxpagesize ? Maxpagesize : value; }
        }

        public string? sort { get; set; }
        public string? categoryCode { get; set; }
        public string? categoryCode2 { get; set; }
        public string? search { get; set; }
        public string? subCategoryCode { get; set; }
        public string? brandCode { get; set; }
        public decimal? minPrice { get; set; }
        public decimal? maxPrice { get; set; }
    }

    public class ProductParams2
    {
        public string? categoryCode { get; set; }
        public string? subCategoryCode { get; set; }
        public string? brandCode { get; set; }

    }
}