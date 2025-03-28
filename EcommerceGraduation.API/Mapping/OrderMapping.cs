using AutoMapper;
using EcommerceGraduation.Core.DTO;
using EcommerceGraduation.Core.Entities;

namespace EcommerceGraduation.API.Mapping
{
    public class OrderMapping : Profile
    {
        public OrderMapping()
        {
            CreateMap<ShippingAdressDTO, Shipping>().ReverseMap();
            CreateMap<OrderDTO, Order>().ReverseMap();

            CreateMap<Order, ReturnOrderDTO>()
                .ForMember(dest => dest.orderDetails, opt => opt.MapFrom(src => src.OrderDetails))
                .ForMember(dest => dest.shipping, opt => opt.MapFrom(src => src.Shippings));

            CreateMap<OrderDetail, ReturnOrderDetailDTO>().ReverseMap();
            CreateMap<Shipping, ReturnOrderShippingDTO>().ReverseMap();
        }
    }

}
