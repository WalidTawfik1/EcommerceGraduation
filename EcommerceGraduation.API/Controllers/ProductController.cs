using AutoMapper;
using EcommerceGraduation.API.Helper;
using EcommerceGraduation.Core.DTO;
using EcommerceGraduation.Core.Interfaces;
using EcommerceGraduation.Core.Sharing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceGraduation.API.Controllers
{

    public class ProductController : BaseController
    {
        public ProductController(IUnitofWork work, IMapper mapper) : base(work, mapper)
        {
        }
        /// <summary>
        /// Get All Products
        /// </summary>
        /// <param name="productParams"></param>
        /// <returns></returns>
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
        /// Get Product By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var product = await work.ProductRepository
                    .GetByIdAsync(id, x => x.CategoryCodeNavigation, x => x.ProductImages,x => x.SubCategoryCodeNavigation, x => x.BrandCodeNavigation);
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
        /// Add Product
        /// </summary>
        /// <param name="productDTO"></param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<IActionResult> Add(AddProductDTO productDTO)
        {
            try
            {

                await work.ProductRepository.AddAsync(productDTO);

                return Ok(new APIResponse(200, "Product added succssfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new APIResponse(400, ex.Message));
            }
        }
        /// <summary>
        /// Update Product
        /// </summary>
        /// <param name="productDTO"></param>
        /// <returns></returns>
        [HttpPut("Update")]
        public async Task<IActionResult> Update(UpdateProductDTO productDTO)
        {
            try
            {
                await work.ProductRepository.UpdateAsync(productDTO);
                return Ok(new APIResponse(200, "Product updated succssfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new APIResponse(400, ex.Message));
            }
        }
        /// <summary>
        /// Delete Product
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var product = await work.ProductRepository
                    .GetByIdAsync(id, x => x.CategoryCodeNavigation, x => x.ProductImages);
                await work.ProductRepository.DeleteAsync(product);
                return Ok(new APIResponse(200, "Product deleted succssfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new APIResponse(400, ex.Message));
            }
        }
    }
}
