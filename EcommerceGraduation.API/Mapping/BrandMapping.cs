using AutoMapper;
using EcommerceGraduation.Core.DTO;
using EcommerceGraduation.Core.Entities;

namespace EcommerceGraduation.API.Mapping
{
    public class BrandMapping: Profile
    {
        public BrandMapping()
        {
            CreateMap<Brand, BrandDTO>().ReverseMap();
            CreateMap<Brand, AddBrandDTO>().ReverseMap();
            CreateMap<Brand, UpdateBrandDTO>().ReverseMap();
        }
    }
}
