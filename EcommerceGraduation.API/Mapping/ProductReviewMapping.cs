using AutoMapper;
using EcommerceGraduation.Core.DTO;
using EcommerceGraduation.Core.Entities;

namespace EcommerceGraduation.API.Mapping
{
    public class ProductReviewMapping:Profile
    {
        public ProductReviewMapping()
        {
            CreateMap<ProductReview,ProductReviewDTO>().ReverseMap();
            CreateMap<ProductReview, ReturnProductReviewDTO>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.CustomerCodeNavigation.Name))
                .ReverseMap();
            CreateMap<UpdateReviewDTO, ProductReview>().ReverseMap();
        }
    }
}
