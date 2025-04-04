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
        public int Maxpagesize { get; set; } = 6;
        private int _pagesize = 3;

        public int pagesize
        {
            get { return _pagesize; }
            set { _pagesize = value > Maxpagesize ? Maxpagesize : value; }
        }
    }
}
