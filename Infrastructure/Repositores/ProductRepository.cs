using AutoMapper;
using Ecom.Core.DTO;
using Ecom.Core.Entities.Product;
using Ecom.Core.Interfaces;
using Ecom.Core.Services;
using Ecom.Core.Sharing;
using Ecom.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Repositores
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly AppDbContext context;
        private readonly IMapper mapper;
        private readonly IImageManagmentService imageManagmentService;

        public ProductRepository(AppDbContext context, IMapper mapper, IImageManagmentService imageManagmentService) : base(context)
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

            var ImagePath =await imageManagmentService.AddImageAsync(productDTO.Photo,productDTO.Name);
            var photo = ImagePath.Select(path => new Photo
            {
                ImageName = path,
                ProductId = product.Id
            }).ToList();
            await context.Photos.AddRangeAsync(photo);
            await context.SaveChangesAsync();
            return true;

        }

        public async Task DeleteAsync(Product product)
        {
            var photo = await context.Photos.Where(i => i.ProductId == product.Id).ToListAsync();
            foreach (var item in photo)
            {
                imageManagmentService.DeleteImageAsync(item.ImageName);
            }
            context.Products.Remove(product); //Photo will deleted automatticly from db "Cascade"
            await context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(UpdateProductDTO productDTO)
        {
            if(productDTO == null) return false;
            var FindProduct = await context.Products
                .Include(m => m.Category)
                .Include(m => m.Photos)
                .FirstOrDefaultAsync(i =>i.Id == productDTO.Id);
            if (FindProduct == null) return false;
            mapper.Map(productDTO, FindProduct);
            var FindPhoto = await context.Photos.Where(i => i.ProductId == productDTO.Id).ToListAsync();
            foreach (var Oldphoto in FindPhoto)
            {
                imageManagmentService.DeleteImageAsync(Oldphoto.ImageName);
            }
            context.Photos.RemoveRange(FindPhoto);
            var ImagePath = await imageManagmentService.AddImageAsync(productDTO.Photo, productDTO.Name);
            var newphoto = ImagePath.Select(path => new Photo
            {
                ImageName = path,
                ProductId = productDTO.Id
            }).ToList();
            await context.Photos.AddRangeAsync(newphoto);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ProductDTO>> GetAllAsync(ProductParams productParams)
        {
            var query =  context.Products
                .Include(m => m.Category)
                .Include(m => m.Photos)
                .AsNoTracking();

            //filter by search
            if (!string.IsNullOrEmpty(productParams.search))
            {
                    var searchword = productParams.search.Split(' ');
                    query = query.Where(m => searchword.All(
                    word => m.Name.ToLower().Contains(word.ToLower())
                    || //or
                    m.Description.ToLower().Contains(word.ToLower())
                    ));
            }



            //filter by categoryId
            if (productParams.categoryId.HasValue)
            {
                query = query.Where(m => m.CategoryId == productParams.categoryId);
            }



            if (!string.IsNullOrEmpty(productParams.sort))
            {
                query = productParams.sort switch
                {
                    "PriceAsc" => query.OrderBy(m => m.NewPrice),
                    "PriceDesc" => query.OrderByDescending(m => m.NewPrice),
                    _ => query.OrderBy(m => m.Name),
                };
            }
            if (productParams.sort == null) query = query.OrderBy(m => m.Name);

            query = query.Skip((productParams.pagenum - 1) * productParams.pagesize).Take(productParams.pagesize);

            var result = mapper.Map<List<ProductDTO>>(query);
            return result;
        }
    }
}
