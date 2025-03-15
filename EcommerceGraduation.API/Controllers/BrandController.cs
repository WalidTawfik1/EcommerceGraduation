using AutoMapper;
using EcommerceGraduation.API.Helper;
using EcommerceGraduation.Core.DTO;
using EcommerceGraduation.Core.Entities;
using EcommerceGraduation.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceGraduation.API.Controllers
{

    public class BrandController : BaseController
    {
        public BrandController(IUnitofWork work, IMapper mapper) : base(work, mapper)
        {
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var brands = await work.BrandRepository.GetAllAsync();
                if (brands == null)
                {
                    return BadRequest(new APIResponse(400));
                }
                return Ok(brands);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetByCode/{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            try
            {
                var brand = await work.BrandRepository.GetByIdAsync(code);
                if (brand == null)
                {
                    return BadRequest(new APIResponse(400, "This Brand Not Found"));
                }
                return Ok(brand);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("Add")]
        public async Task<IActionResult> Add(AddBrandDTO addBrandDTO)
        {
            try
            {
                var brand = mapper.Map<Brand>(addBrandDTO);
                await work.BrandRepository.AddAsync(brand);
                return Ok(new APIResponse(200, "Brand added succssfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update(UpdateBrandDTO updateBrandDTO)
        {
            try
            {
                var brand = mapper.Map<Brand>(updateBrandDTO);
                await work.BrandRepository.UpdateAsync(brand);
                return Ok(new APIResponse(200, "Brand updated succssfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("Delete/{code}")]
        public async Task<IActionResult> Delete(string code)
        {
            try
            {
                await work.BrandRepository.DeleteAsync(code);
                return Ok(new APIResponse(200, "Brand deleted succssfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    

}
}
