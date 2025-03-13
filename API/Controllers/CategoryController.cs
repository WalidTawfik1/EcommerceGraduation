using AutoMapper;
using Ecom.API.Helper;
using Ecom.Core.DTO;
using Ecom.Core.Entities.Product;
using Ecom.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecom.API.Controllers
{
    public class CategoryController : BaseController
    {
        public CategoryController(IUnitofWork work, IMapper mapper) : base(work, mapper)
        {
        }

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
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var category = await work.CategoryRepository.GetByIdAsync(id);
                if (category == null)
                {
                    return BadRequest(new APIResponse(400,"This Category Not Found"));
                }
                return Ok(category);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("Add")]
        public async Task<IActionResult> Add(CategoryDTO categoryDTO)
        {
            try
            {
                var category = mapper.Map<Category>(categoryDTO);
                await work.CategoryRepository.AddAsync(category);
                return Ok(new APIResponse(200,"Category added succssfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("Update")]
        public async Task<IActionResult> Update(UpdateCategoryDTO updatecategoryDTO)
        {
            try
            {
                var category = mapper.Map<Category>(updatecategoryDTO);
                await work.CategoryRepository.UpdateAsync(category);
                return Ok(new APIResponse(200, "Category updated succssfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await work.CategoryRepository.DeleteAsync(id);
                return Ok(new APIResponse(200, "Category removed succssfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
