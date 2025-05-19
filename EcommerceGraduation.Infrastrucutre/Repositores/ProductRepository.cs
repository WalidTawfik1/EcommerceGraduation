using AutoMapper;
using EcommerceGraduation.Core.DTO;
using EcommerceGraduation.Core.Entities;
using EcommerceGraduation.Core.Interfaces;
using EcommerceGraduation.Core.Services;
using EcommerceGraduation.Core.Sharing;
using EcommerceGraduation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceGraduation.Infrastrucutre.Repositores
{
    public class ProductRepository : GenericRepository<Product,int>, IProductRepository
    {
        private readonly EcommerceDbContext context;
        private readonly IMapper mapper;
        private readonly IProductImageManagmentService imageManagmentService;
        public ProductRepository(EcommerceDbContext context, IMapper mapper, IProductImageManagmentService imageManagmentService) : base(context)
        {
            this.context = context;
            this.mapper = mapper;
            this.imageManagmentService = imageManagmentService;
        }

        public async Task<bool> AddAsync(AddProductDTO productDTO)
        {
            if (productDTO == null) return false;
            var product = mapper.Map<Product>(productDTO);
            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            var ImagePath = await imageManagmentService.AddImageAsync(productDTO.Photo, productDTO.Name);
            var photo = ImagePath.Select(path => new ProductImage
            {
                ImageUrl = path,
                ProductId = product.ProductId
            }).ToList();
            await context.ProductImages.AddRangeAsync(photo);
            await context.SaveChangesAsync();
            return true;

        }

        public async Task DeleteAsync(Product product)
        {
            var photo = await context.ProductImages.Where(i => i.ProductId == product.ProductId).ToListAsync();
            foreach (var item in photo)
            {
                imageManagmentService.DeleteImageAsync(item.ImageUrl);
            }
            context.Products.Remove(product); //Photo will deleted automatticly from db "Cascade"
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProductDTO>> GetAllAsync(ProductParams productParams)
        {
            var query = context.Products
                         .Include(m => m.CategoryCodeNavigation)
                         .Include(m => m.SubCategoryCodeNavigation)
                         .Include(m => m.BrandCodeNavigation)
                         .Include(m => m.ProductImages)
                         .Include(m => m.ProductReviews).ThenInclude(m => m.CustomerCodeNavigation)
                         .AsNoTracking();

            //filter by search
            if (!string.IsNullOrEmpty(productParams.search))
            {
                var searchwords = productParams.search.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (searchwords.Any())
                {
                    // Match ANY of the search words instead of ALL
                    query = query.Where(m => searchwords.Any(
                        word => m.Name.ToLower().Contains(word.ToLower())
                        ||
                        m.Description.ToLower().Contains(word.ToLower())
                    ));
                }
            }



            //filter by categoryId
            if (!string.IsNullOrEmpty(productParams.categoryCode))
            {
                //filter by two categoryIds
                if (!string.IsNullOrEmpty(productParams.categoryCode) && !string.IsNullOrEmpty(productParams.categoryCode2))
                {
                    query = query.Where(m => m.CategoryCode == productParams.categoryCode || m.CategoryCode == productParams.categoryCode2);
                }
                else
                {
                    query = query.Where(m => m.CategoryCode == productParams.categoryCode);
                }

            }

            

            //filter by subcategoryId
            if (!string.IsNullOrEmpty(productParams.subCategoryCode))
            {
                query = query.Where(m => m.SubCategoryCode == productParams.subCategoryCode);
            }

            //filter by brandId
            if (!string.IsNullOrEmpty(productParams.brandCode))
            {
                query = query.Where(m => m.BrandCode == productParams.brandCode);
            }

            //filter by price
            if (productParams.minPrice != null && productParams.maxPrice != null)
            {
                query = query.Where(m => m.Price >= productParams.minPrice && m.Price <= productParams.maxPrice);
            }
            else if (productParams.minPrice != null)
            {
                query = query.Where(m => m.Price >= productParams.minPrice);
            }
            else if (productParams.maxPrice != null)
            {
                query = query.Where(m => m.Price <= productParams.maxPrice);
            }



            if (!string.IsNullOrEmpty(productParams.sort))
            {
                query = productParams.sort switch
                {
                    "PriceAsc" => query.OrderBy(m => m.Price),
                    "PriceDesc" => query.OrderByDescending(m => m.Price),
                    "Rating" => query.OrderByDescending(m => m.Rating),
                    _ => query.OrderBy(m => m.Name),
                };
            }
            if (productParams.sort == null) query = query.OrderBy(m => m.Name);

            query = query.Skip((productParams.pagenum - 1) * productParams.pagesize).Take(productParams.pagesize);

            var result = mapper.Map<List<ProductDTO>>(query);
            return result;
        }

        public async Task<IEnumerable<ProductDTO>> GetAllNoPaginateAsync(ProductParams2 productParams2)
        {
            var query = context.Products
                         .Include(m => m.CategoryCodeNavigation)
                         .Include(m => m.SubCategoryCodeNavigation)
                         .Include(m => m.BrandCodeNavigation)
                         .Include(m => m.ProductImages)
                         .Include(m => m.ProductReviews).ThenInclude(m => m.CustomerCodeNavigation)
                         .AsNoTracking();

            //filter by categoryId
            if (!string.IsNullOrEmpty(productParams2.categoryCode))
            {
                query = query.Where(m => m.CategoryCode == productParams2.categoryCode);
            }

            //filter by subcategoryId
            if (!string.IsNullOrEmpty(productParams2.subCategoryCode))
            {
                query = query.Where(m => m.SubCategoryCode == productParams2.subCategoryCode);
            }

            //filter by brandId
            if (!string.IsNullOrEmpty(productParams2.brandCode))
            {
                query = query.Where(m => m.BrandCode == productParams2.brandCode);
            }

            var result = mapper.Map<List<ProductDTO>>(query);
            return result;
        }

        public async Task<bool> UpdateAsync(UpdateProductDTO productDTO)
        {
            if (productDTO == null) return false;
            var FindProduct = await context.Products
                .Include(m => m.CategoryCodeNavigation)
                .Include(m => m.SubCategoryCodeNavigation)
                .Include(m => m.BrandCodeNavigation)
                .Include(m => m.ProductImages)
                .FirstOrDefaultAsync(i => i.ProductId == productDTO.ProductId);
            if (FindProduct == null) return false;
            mapper.Map(productDTO, FindProduct);
            var FindPhoto = await context.ProductImages.Where(i => i.ProductId == productDTO.ProductId).ToListAsync();
            foreach (var Oldphoto in FindPhoto)
            {
                imageManagmentService.DeleteImageAsync(Oldphoto.ImageUrl);
            }
            context.ProductImages.RemoveRange(FindPhoto);
            var ImagePath = await imageManagmentService.AddImageAsync(productDTO.Photo, productDTO.Name);
            var newphoto = ImagePath.Select(path => new ProductImage
            {
                ImageUrl = path,
                ProductId = productDTO.ProductId
            }).ToList();
            await context.ProductImages.AddRangeAsync(newphoto);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
