using AutoMapper;
using EcommerceGraduation.API.Helper;
using EcommerceGraduation.Core.DTO;
using EcommerceGraduation.Core.Interfaces;
using EcommerceGraduation.Core.Sharing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceGraduation.API.Controllers
{
    /// <summary>
    /// Controller for managing products.
    /// </summary>
    public class ProductController : BaseController
    {
        public ProductController(IUnitofWork work, IMapper mapper) : base(work, mapper)
        {
        }

        /// <summary>
        /// Gets all products with pagination.
        /// </summary>
        /// <param name="productParams">Parameters for pagination and filtering.</param>
        /// <returns>A paginated list of products.</returns>
        [AllowAnonymous]
        [HttpGet("get-all-products")]
        public async Task<IActionResult> GetAll([FromQuery] ProductParams productParams)
        {
            try
            {
                var products = await work.ProductRepository.GetAllAsync(productParams);
                var totalcount = await work.ProductRepository.CountAsync();
                return Ok(new Pagination<ProductDTO>(productParams.pagenum, productParams.pagesize, totalcount, products));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Gets a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product.</param>
        /// <returns>The product with the specified ID.</returns>
        [AllowAnonymous]
        [HttpGet("get-product-by-id/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var product = await work.ProductRepository
                    .GetByIdAsync(id,
                    x => x.CategoryCodeNavigation,
                    x => x.ProductImages,
                    x => x.SubCategoryCodeNavigation,
                    x => x.BrandCodeNavigation,
                    x => x.ProductReviews);
                var result = mapper.Map<ProductDTO>(product);
                if (product == null)
                {
                    return BadRequest(new APIResponse(400, "This Product Not Found"));
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Gets all products without pagination.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("get-all-products-no-paginate")]
        public async Task<IActionResult> GetAllNoPaginate([FromQuery] ProductParams2 productParams2)
        {
            try
            {
                var products = await work.ProductRepository.GetAllNoPaginateAsync(productParams2);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Adds a new product.
        /// </summary>
        /// <param name="productDTO">The product details.</param>
        /// <returns>A response indicating the result of the addition.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("add-product")]
        public async Task<IActionResult> Add([FromForm] AddProductDTO productDTO)
        {
            try
            {
                await work.ProductRepository.AddAsync(productDTO);
                return Ok(new APIResponse(200, "Product added successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new APIResponse(400, ex.Message));
            }
        }

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        /// <param name="productDTO">The product details.</param>
        /// <returns>A response indicating the result of the update.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("update-product")]
        public async Task<IActionResult> Update([FromForm] UpdateProductDTO productDTO)
        {
            try
            {
                await work.ProductRepository.UpdateAsync(productDTO);
                return Ok(new APIResponse(200, "Product updated successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new APIResponse(400, ex.Message));
            }
        }

        /// <summary>
        /// Deletes a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product.</param>
        /// <returns>A response indicating the result of the deletion.</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("delete-product/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var product = await work.ProductRepository
                    .GetByIdAsync(id, x => x.CategoryCodeNavigation, x => x.ProductImages);
                await work.ProductRepository.DeleteAsync(product);
                return Ok(new APIResponse(200, "Product deleted successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new APIResponse(400, ex.Message));
            }
        }
    }
}
