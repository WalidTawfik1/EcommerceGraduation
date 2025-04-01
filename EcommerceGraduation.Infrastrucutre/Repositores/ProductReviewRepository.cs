using EcommerceGraduation.Core.DTO;
using EcommerceGraduation.Core.Entities;
using EcommerceGraduation.Core.Interfaces;
using EcommerceGraduation.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceGraduation.Infrastrucutre.Repositores
{
    public class ProductReviewRepository : IProductReview
    {
        private readonly EcommerceDbContext _context;
        private readonly UserManager<Customer> _userManager;

        public ProductReviewRepository(EcommerceDbContext context, UserManager<Customer> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<bool> AddRatingAsync(ProductReviewDTO reviewDTO, string CustomerCode)
        {
            var customer = await _userManager.FindByIdAsync(CustomerCode);
            if (await _context.ProductReviews.AsNoTracking().AnyAsync(m => m.CustomerCode == customer.Id && m.ProductId == reviewDTO.ProductId))
            {
                return false;
            }
            var review = new ProductReview
            {
                CustomerCode = customer.Id,
                ProductId = reviewDTO.ProductId,
                Rating = reviewDTO.Rating,
                ReviewText = reviewDTO.ReviewText,
            };
            await _context.ProductReviews.AddAsync(review);
            await _context.SaveChangesAsync();

            var product = await _context.Products.FirstOrDefaultAsync(m => m.ProductId == reviewDTO.ProductId);
            var ratings = await _context.ProductReviews.AsNoTracking().Where(m => m.ProductId == product.ProductId).ToListAsync();

            if(ratings.Count > 0)
            {
                double? average = ratings.Average(m => m.Rating);
                double roundedReview = Math.Round((double)(average * 2), mode: MidpointRounding.AwayFromZero) / 2;
                product.Rating = roundedReview;
            }
            else
            {
                product.Rating = reviewDTO.Rating;
            }
            await _context.SaveChangesAsync();
            return true;

        }

        public async Task<IReadOnlyList<ReturnProductReviewDTO>> GetAllRatingForProductAsync(int productId)
        {
            var ratings = await _context.ProductReviews.Include(m => m.CustomerCodeNavigation).AsNoTracking().Where(m => m.ProductId == productId).ToListAsync();

            return ratings.Select(m => new ReturnProductReviewDTO
            {
                CustomerName = m.CustomerCodeNavigation.Name,
                Rating = (int)m.Rating,
                ReviewText = m.ReviewText,
                ReviewDate = m.ReviewDate.Value
            }).ToList();
        }
    }
}
