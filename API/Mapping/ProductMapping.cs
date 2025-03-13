using AutoMapper;
using Ecom.Core.DTO;
using Ecom.Core.Entities.Product;
namespace Ecom.API.Mapping
{
    public class ProductMapping : Profile
    {
        public ProductMapping()
        {
            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name)).ReverseMap();
            CreateMap<Photo, PhotoDTO>().ReverseMap();
            CreateMap<Product, AddProductDTO>()
                .ForMember(p =>p.Photo, memberOptions: opt => opt.Ignore()).ReverseMap();
            CreateMap<Product, UpdateProductDTO>()
                 .ForMember(p => p.Photo, memberOptions: opt => opt.Ignore()).ReverseMap();

        }
    }
}
