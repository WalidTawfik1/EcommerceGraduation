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
                .ForMember(dest => dest.ReviewId, opt => opt.MapFrom(src => src.ReviewId))
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerCodeNavigation.Id))
                .ReverseMap();
            CreateMap<UpdateReviewDTO, ProductReview>().ReverseMap();
        }
    }
}
