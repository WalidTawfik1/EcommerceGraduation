using AutoMapper;
using EcommerceGraduation.API.Helper;
using EcommerceGraduation.Core.DTO;
using EcommerceGraduation.Core.Entities;
using EcommerceGraduation.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceGraduation.API.Controllers
{

    public class SubCategoryController : BaseController
    {
        public SubCategoryController(IUnitofWork work, IMapper mapper) : base(work, mapper)
        {
        }

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
        [HttpPost("Add")]
        public async Task<IActionResult> Add(AddSubCategoryDTO addSubCategoryDTO)
        {
            try
            {
                var subCategory = mapper.Map<SubCategory>(addSubCategoryDTO);
                await work.SubCategoryRepository.AddAsync(subCategory);
                return Ok(new APIResponse(200, "SubCategory added succssfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update(UpdateSubCategoryDTO updateSubCategoryDTO)
        {
            try
            {
                var subCategory = mapper.Map<SubCategory>(updateSubCategoryDTO);
                await work.SubCategoryRepository.UpdateAsync(subCategory);
                return Ok(new APIResponse(200, "SubCategory updated succssfully"));
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
                await work.SubCategoryRepository.DeleteAsync(code);
                return Ok(new APIResponse(200, "SubCategory deleted succssfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}