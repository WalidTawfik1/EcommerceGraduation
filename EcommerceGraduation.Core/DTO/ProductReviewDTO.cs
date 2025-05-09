﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceGraduation.Core.DTO
{
    public class ProductReviewDTO
    {
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string? ReviewText { get; set; }
    }

    public class ReturnProductReviewDTO
    {
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int ReviewId { get; set; }
        public int Rating { get; set; }
        public string? ReviewText { get; set; }
        public DateTime ReviewDate { get; set; }
    }

    public class  UpdateReviewDTO
    {
        public int ReviewId { get; set; }
        public int Rating { get; set; }
        public string? ReviewText { get; set; }
    }

}
