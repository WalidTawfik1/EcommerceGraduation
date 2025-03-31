using AutoMapper;
using EcommerceGraduation.Core.DTO;
using EcommerceGraduation.Core.Entities;

namespace EcommerceGraduation.API.Mapping
{
    public class InvoiceMapping:Profile
    {
        public InvoiceMapping()
        {
            CreateMap<Invoice, InvoiceDTO>().ReverseMap();
        }
    }
}
