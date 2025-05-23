﻿using EcommerceGraduation.Core.DTO;
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
                double roundedReview = Math.Round((double)average, 1);
                product.Rating = (double)roundedReview;
            }
            else
            {
                product.Rating = (double)reviewDTO.Rating;
            }
            await _context.SaveChangesAsync();
            return true;

        }

        public async Task<bool> DeleteReviewAsync(int reviewId, string CustomerCode)
        {
            var customer = await _userManager.FindByIdAsync(CustomerCode);
            var review = await _context.ProductReviews.FirstOrDefaultAsync(m => m.ReviewId == reviewId && m.CustomerCode == customer.Id);

            if (review == null)
            {
                return false; 
            }

            _context.ProductReviews.Remove(review);
            await _context.SaveChangesAsync();

            var product = await _context.Products.FirstOrDefaultAsync(m => m.ProductId == review.ProductId);
            var ratings = await _context.ProductReviews.AsNoTracking().Where(m => m.ProductId == product.ProductId).ToListAsync();

            if (ratings.Count > 0)
            {
                double? average = ratings.Average(m => m.Rating);
                double roundedReview = Math.Round((double)average, 1);
                product.Rating = roundedReview;
            }
            else
            {
                product.Rating = null;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IReadOnlyList<ReturnProductReviewDTO>> GetAllRatingForProductAsync(int productId)
        {
            var ratings = await _context.ProductReviews.Include(m => m.CustomerCodeNavigation).AsNoTracking().Where(m => m.ProductId == productId).ToListAsync();

            return ratings.Select(m => new ReturnProductReviewDTO
            {
                CustomerId = m.CustomerCode,
                ReviewId = m.ReviewId,
                CustomerName = m.CustomerCodeNavigation.Name,
                Rating = (int)m.Rating,
                ReviewText = m.ReviewText,
                ReviewDate = m.ReviewDate.Value
            }).ToList();
        }

        public async Task<bool> UpdateReviewAsync(UpdateReviewDTO updateReviewDTO, string CustomerCode)
        {
            var customer = await _userManager.FindByIdAsync(CustomerCode);
            var review = await _context.ProductReviews.FirstOrDefaultAsync(m => m.ReviewId == updateReviewDTO.ReviewId && m.CustomerCode == customer.Id);

            if (review == null)
            {
                return false;
            }

            review.Rating = updateReviewDTO.Rating;
            if (!string.IsNullOrWhiteSpace(updateReviewDTO.ReviewText))
            {
                review.ReviewText = updateReviewDTO.ReviewText;
            }

            _context.ProductReviews.Update(review);
            await _context.SaveChangesAsync();

            // Recalculate product rating
            var product = await _context.Products.FirstOrDefaultAsync(m => m.ProductId == review.ProductId);
            var ratings = await _context.ProductReviews.AsNoTracking().Where(m => m.ProductId == product.ProductId).ToListAsync();

            product.Rating = ratings.Any()
                 ? Math.Round((double)ratings.Average(m => m.Rating), 1)
                : Math.Round((double)updateReviewDTO.Rating, 1);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> CountReviewsForProductAsync(int productId)
        {
            return await _context.ProductReviews.CountAsync(m => m.ProductId == productId);
        }
    }
}
