using AutoMapper;
using EcommerceGraduation.Core.DTO;
using EcommerceGraduation.Core.Entities;

namespace EcommerceGraduation.API.Mapping
{
    public class CategoryMapping : Profile
    {
        public CategoryMapping()
        {
            CreateMap<CategoryDTO, Category>().ReverseMap();
            CreateMap<UpdateCategoryDTO, Category>().ReverseMap();
            CreateMap<AddCategoryDTO, Category>().ReverseMap();
        }
    }
}
