using AutoMapper;
using EcommerceGraduation.Core.DTO;
using EcommerceGraduation.Core.Entities;

namespace EcommerceGraduation.API.Mapping
{
    public class SubCategoryMapping : Profile
    {
        public SubCategoryMapping()
        {
            CreateMap<SubCategoryDTO, SubCategory>().ReverseMap();
            CreateMap<UpdateSubCategoryDTO, SubCategory>().ReverseMap();
            CreateMap<AddSubCategoryDTO, SubCategory>().ReverseMap();
        }
    }
}
