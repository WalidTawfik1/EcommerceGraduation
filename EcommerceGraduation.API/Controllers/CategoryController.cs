using AutoMapper;
using EcommerceGraduation.API.Helper;
using EcommerceGraduation.Core.DTO;
using EcommerceGraduation.Core.Entities;
using EcommerceGraduation.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceGraduation.API.Controllers
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
        [HttpGet("GetByCode/{code}")]
        public async Task<IActionResult> GetById(string code)
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
        [HttpPost("Add")]
        public async Task<IActionResult> Add(AddCategoryDTO addcategoryDTO)
        {
            try
            {
                addcategoryDTO = addcategoryDTO with { CategoryCode = AddCategoryDTO.GenerateCategoryCode() };
                var category = mapper.Map<Category>(addcategoryDTO);
                await work.CategoryRepository.AddAsync(category);
                return Ok(new APIResponse(200, "Category added succssfully"));
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
        [HttpDelete("Delete/{code}")]
        public async Task<IActionResult> Delete(string code)
        {
            try
            {
                await work.CategoryRepository.DeleteAsync(code);
                return Ok(new APIResponse(200, "Category removed succssfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
