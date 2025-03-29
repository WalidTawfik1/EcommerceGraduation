using AutoMapper;
using EcommerceGraduation.API.Helper;
using EcommerceGraduation.Core.DTO;
using EcommerceGraduation.Core.Entities;
using EcommerceGraduation.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceGraduation.API.Controllers
{
    /// <summary>
    /// Controller for managing brands.
    /// </summary>
    public class BrandController : BaseController
    {
        public BrandController(IUnitofWork work, IMapper mapper) : base(work, mapper)
        {
        }

        /// <summary>
        /// Gets all brands.
        /// </summary>
        /// <returns>A list of all brands.</returns>
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

        /// <summary>
        /// Gets a brand by its code.
        /// </summary>
        /// <param name="code">The brand code.</param>
        /// <returns>The brand with the specified code.</returns>
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

        /// <summary>
        /// Adds a new brand.
        /// </summary>
        /// <param name="addBrandDTO">The brand details.</param>
        /// <returns>A response indicating the result of the addition.</returns>
        [HttpPost("Add")]
        public async Task<IActionResult> Add(AddBrandDTO addBrandDTO)
        {
            try
            {
                var brand = mapper.Map<Brand>(addBrandDTO);
                await work.BrandRepository.AddAsync(brand);
                return Ok(new APIResponse(200, "Brand added successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Updates an existing brand.
        /// </summary>
        /// <param name="updateBrandDTO">The brand details.</param>
        /// <returns>A response indicating the result of the update.</returns>
        [HttpPut("Update")]
        public async Task<IActionResult> Update(UpdateBrandDTO updateBrandDTO)
        {
            try
            {
                var brand = mapper.Map<Brand>(updateBrandDTO);
                await work.BrandRepository.UpdateAsync(brand);
                return Ok(new APIResponse(200, "Brand updated successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a brand by its code.
        /// </summary>
        /// <param name="code">The brand code.</param>
        /// <returns>A response indicating the result of the deletion.</returns>
        [HttpDelete("Delete/{code}")]
        public async Task<IActionResult> Delete(string code)
        {
            try
            {
                await work.BrandRepository.DeleteAsync(code);
                return Ok(new APIResponse(200, "Brand deleted successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
