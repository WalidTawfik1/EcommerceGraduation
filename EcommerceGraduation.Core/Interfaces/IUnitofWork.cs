﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceGraduation.Core.Interfaces
{
    public interface IUnitofWork
    {
        IProductRepository ProductRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        ICartRepository CartRepository { get; }
        IProductImageRepository ProductImageRepository { get; }
        ISubCategoryRepository SubCategoryRepository { get; }
        IBrandRepository BrandRepository { get; }
        IAuthentication Authentication { get; }
        ICustomerRepository CustomerRepository { get; }
        IWishlistRepository WishlistRepository { get; }
    }
}
