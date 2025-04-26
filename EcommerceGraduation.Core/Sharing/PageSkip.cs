using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceGraduation.Core.Sharing
{
    public class PageSkip
    {
        public int pagenum { get; set; } = 1;
        public int Maxpagesize { get; set; } = 50;
        private int _pagesize;
        public string? search { get; set; }
        public string? sort { get; set; }



        public int pagesize
        {
            get { return _pagesize; }
            set { _pagesize = value > Maxpagesize ? Maxpagesize : value; }
        }
    }
}
