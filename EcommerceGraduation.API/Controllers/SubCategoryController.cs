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
    /// Controller for managing subcategories.
    /// </summary>
    public class SubCategoryController : BaseController
    {
        public SubCategoryController(IUnitofWork work, IMapper mapper) : base(work, mapper)
        {
        }

        /// <summary>
        /// Gets all subcategories.
        /// </summary>
        /// <returns>A list of all subcategories.</returns>
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var subCategories = await work.SubCategoryRepository.GetAllAsync();
                if (subCategories == null)
                {
                    return BadRequest(new APIResponse(400));
                }
                return Ok(subCategories);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Gets a subcategory by its code.
        /// </summary>
        /// <param name="code">The subcategory code.</param>
        /// <returns>The subcategory with the specified code.</returns>
        [HttpGet("GetByCode/{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            try
            {
                var subCategory = await work.SubCategoryRepository.GetByIdAsync(code);
                if (subCategory == null)
                {
                    return BadRequest(new APIResponse(400, "This SubCategory Not Found"));
                }
                return Ok(subCategory);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Adds a new subcategory.
        /// </summary>
        /// <param name="addSubCategoryDTO">The subcategory details.</param>
        /// <returns>A response indicating the result of the addition.</returns>
        [HttpPost("Add")]
        public async Task<IActionResult> Add(AddSubCategoryDTO addSubCategoryDTO)
        {
            try
            {
                var subCategory = mapper.Map<SubCategory>(addSubCategoryDTO);
                await work.SubCategoryRepository.AddAsync(subCategory);
                return Ok(new APIResponse(200, "SubCategory added successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Updates an existing subcategory.
        /// </summary>
        /// <param name="updateSubCategoryDTO">The subcategory details.</param>
        /// <returns>A response indicating the result of the update.</returns>
        [HttpPut("Update")]
        public async Task<IActionResult> Update(UpdateSubCategoryDTO updateSubCategoryDTO)
        {
            try
            {
                var subCategory = mapper.Map<SubCategory>(updateSubCategoryDTO);
                await work.SubCategoryRepository.UpdateAsync(subCategory);
                return Ok(new APIResponse(200, "SubCategory updated successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a subcategory by its code.
        /// </summary>
        /// <param name="code">The subcategory code.</param>
        /// <returns>A response indicating the result of the deletion.</returns>
        [HttpDelete("Delete/{code}")]
        public async Task<IActionResult> Delete(string code)
        {
            try
            {
                await work.SubCategoryRepository.DeleteAsync(code);
                return Ok(new APIResponse(200, "SubCategory deleted successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
