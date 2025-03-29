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
    /// Controller for managing categories.
    /// </summary>
    public class CategoryController : BaseController
    {
        public CategoryController(IUnitofWork work, IMapper mapper) : base(work, mapper)
        {
        }

        /// <summary>
        /// Gets all categories.
        /// </summary>
        /// <returns>A list of all categories.</returns>
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var categories = await work.CategoryRepository.GetAllAsync();
                if (categories == null)
                {
                    return BadRequest(new APIResponse(400));
                }
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Gets a category by its code.
        /// </summary>
        /// <param name="code">The category code.</param>
        /// <returns>The category with the specified code.</returns>
        [HttpGet("GetByCode/{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            try
            {
                var category = await work.CategoryRepository.GetByIdAsync(code);
                if (category == null)
                {
                    return BadRequest(new APIResponse(400, "This Category Not Found"));
                }
                return Ok(category);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Adds a new category.
        /// </summary>
        /// <param name="addCategoryDTO">The category details.</param>
        /// <returns>A response indicating the result of the addition.</returns>
        [HttpPost("Add")]
        public async Task<IActionResult> Add(AddCategoryDTO addCategoryDTO)
        {
            try
            {
                var category = mapper.Map<Category>(addCategoryDTO);
                await work.CategoryRepository.AddAsync(category);
                return Ok(new APIResponse(200, "Category added successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Updates an existing category.
        /// </summary>
        /// <param name="updateCategoryDTO">The category details.</param>
        /// <returns>A response indicating the result of the update.</returns>
        [HttpPut("Update")]
        public async Task<IActionResult> Update(UpdateCategoryDTO updateCategoryDTO)
        {
            try
            {
                var category = mapper.Map<Category>(updateCategoryDTO);
                await work.CategoryRepository.UpdateAsync(category);
                return Ok(new APIResponse(200, "Category updated successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a category by its code.
        /// </summary>
        /// <param name="code">The category code.</param>
        /// <returns>A response indicating the result of the deletion.</returns>
        [HttpDelete("Delete/{code}")]
        public async Task<IActionResult> Delete(string code)
        {
            try
            {
                await work.CategoryRepository.DeleteAsync(code);
                return Ok(new APIResponse(200, "Category removed successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
