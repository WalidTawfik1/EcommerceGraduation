using EcommerceGraduation.Core.DTO;
using EcommerceGraduation.Core.Entities;
using EcommerceGraduation.Core.Sharing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceGraduation.Core.Services
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(OrderDTO orderDTO,string CustomerCode);
        Task<IReadOnlyList<ReturnOrderDTO>> GetAllOrdersForUserAsync(string CustomerCode);
        Task<ReturnOrderDTO> GetOrderByIdAsync(string orderNumber,string CustomerCode);
        Task<IReadOnlyList<ReturnOrderDTO>> GetAllOrders(PageSkip page);
        Task<IReadOnlyList<ReturnOrderDTO>> GetAllOrdersNoPaginate();
        Task<ReturnOrderStatusDTO> GetOrderStatusByIdAsync(string orderNumber);


    }
}
