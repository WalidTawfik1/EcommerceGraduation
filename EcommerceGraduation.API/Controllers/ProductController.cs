using AutoMapper;
using EcommerceGraduation.API.Helper;
using EcommerceGraduation.Core.DTO;
using EcommerceGraduation.Core.Interfaces;
using EcommerceGraduation.Core.Sharing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        [HttpGet("GetAll")]
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
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var product = await work.ProductRepository
                    .GetByIdAsync(id, x => x.CategoryCodeNavigation, x => x.ProductImages, x => x.SubCategoryCodeNavigation, x => x.BrandCodeNavigation);
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
        /// Adds a new product.
        /// </summary>
        /// <param name="productDTO">The product details.</param>
        /// <returns>A response indicating the result of the addition.</returns>
        [HttpPost("Add")]
        public async Task<IActionResult> Add(AddProductDTO productDTO)
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
        [HttpPut("Update")]
        public async Task<IActionResult> Update(UpdateProductDTO productDTO)
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
        [HttpDelete("Delete/{id}")]
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
