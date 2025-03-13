using AutoMapper;
using Ecom.API.Helper;
using Ecom.Core.DTO;
using Ecom.Core.Entities.Product;
using Ecom.Core.Interfaces;
using Ecom.Core.Sharing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecom.API.Controllers
{
    public class ProductController : BaseController
    {
        public ProductController(IUnitofWork work, IMapper mapper) : base(work, mapper)
        {
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery]ProductParams productParams)
        {
            try
            {
                var products = await work.ProductRepository.GetAllAsync(productParams);
                var totalcount = await work.ProductRepository.CountAsync();
                return Ok(new Pagination<ProductDTO>(productParams.pagenum,productParams.pagesize,totalcount,products));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var product = await work.ProductRepository
                    .GetByIdAsync(id, x => x.Category, x => x.Photos);
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
                return BadRequest(new APIResponse (400,ex.Message));
            }
        }
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
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var product = await work.ProductRepository
                    .GetByIdAsync(id, x => x.Category, x => x.Photos);
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
