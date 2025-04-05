using AutoMapper;
using EcommerceGraduation.Core.DTO;
using EcommerceGraduation.Core.Entities;
namespace EcommerceGraduation.API.Mapping
{
    public class ProductMapping : Profile
    {
        public ProductMapping()
        {
            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.CategoryCodeNavigation.Name))
                .ForMember(dest => dest.SubCategoryName, opt => opt.MapFrom(src => src.SubCategoryCodeNavigation.Name))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.BrandCodeNavigation.BrandName))
                .ForMember(dest => dest.Photos, opt => opt.MapFrom(src => src.ProductImages))
                .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => src.ProductReviews.Count))
                .ForMember(dest => dest.Reviews, opt => opt.MapFrom(src => src.ProductReviews))
                .ReverseMap();
            CreateMap<ProductImage, PhotoDTO>().ReverseMap();
            CreateMap<Product, AddProductDTO>()
                .ForMember(p => p.Photo, memberOptions: opt => opt.Ignore()).ReverseMap()
                .ForMember(p => p.Barcode, opt => opt.Ignore());
            CreateMap<Product, UpdateProductDTO>()
                 .ForMember(p => p.Photo, memberOptions: opt => opt.Ignore()).ReverseMap()
                 .ForMember(p => p.Barcode, opt => opt.Ignore());
            CreateMap<ProductReview, ReviewDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CustomerCodeNavigation.Name))
                .ReverseMap();




        }
    }
}
