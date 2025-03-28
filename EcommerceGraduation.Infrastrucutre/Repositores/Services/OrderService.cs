using AutoMapper;
using EcommerceGraduation.Core.DTO;
using EcommerceGraduation.Core.Entities;
using EcommerceGraduation.Core.Interfaces;
using EcommerceGraduation.Core.Services;
using EcommerceGraduation.Core.Sharing;
using EcommerceGraduation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceGraduation.Infrastrucutre.Repositores.Services
{
    
    public class OrderService : IOrderService
    {
        private readonly IUnitofWork _unitofWork;
        private readonly EcommerceDbContext _context;
        private readonly IMapper _mapper;

        public OrderService(IUnitofWork unitofWork, EcommerceDbContext context, IMapper mapper)
        {
            _unitofWork = unitofWork;
            _context = context;
            _mapper = mapper;
        }

        public async Task<Order> CreateOrderAsync(OrderDTO orderDTO, string CustomerCode)
        {
            var cart = await _unitofWork.CartRepository.GetCartAsync(orderDTO.CartId);
            if (cart == null || !cart.Items.Any())
            {
                throw new InvalidOperationException("Cart is empty or does not exist.");
            }

            var order = _mapper.Map<Order>(orderDTO);
            order.CustomerCode = CustomerCode;
            order.TotalAmount = cart.SubAmount;

            var orderDetails = new List<OrderDetail>();
            foreach (var item in cart.Items)
            {
                var product = await _unitofWork.ProductRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                {
                    throw new InvalidOperationException($"Product with ID {item.ProductId} not found.");
                }

                var orderDetail = new OrderDetail
                {
                    ProductId = item.ProductId,
                    ProductName = product.Name,
                    MainImage = item.Photo,
                    Quantity = item.Quantity,
                    Price = product.Price,
                    SubTotal = (double)cart.SubAmount,
                    OrderNumber = order.OrderNumber
                };
                orderDetails.Add(orderDetail);
            }
            order.OrderDetails = orderDetails;


            var shipping = _mapper.Map<Shipping>(orderDTO.ShippingDTO);
            decimal shippingCost = 0m;
            if (shipping.ShippingMethod == ShippingMethod.Standard)
            {
                shipping.EstimatedDeliveryDate = DateOnly.FromDateTime(DateTime.Now.AddDays(4));
                shippingCost = 20m;
            }
            else if (shipping.ShippingMethod == ShippingMethod.Express)
            {
                shipping.EstimatedDeliveryDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2));
                shippingCost = 40m;
            }
            else
            {
                shipping.EstimatedDeliveryDate = DateOnly.FromDateTime(DateTime.Now.AddDays(4));
                shippingCost = 20m;
            }
                shipping.TrackingNumber = Guid.NewGuid().ToString();
            await _context.Shippings.AddAsync(shipping);
            await _context.SaveChangesAsync();

            shipping.TrackingNumber = GenerateCode.GenerateTrackingNumber(shipping.ShippingId);
            _context.Shippings.Update(shipping);

            shipping.OrderNumber = order.OrderNumber;

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            order.TotalAmount += shippingCost;
            await _context.SaveChangesAsync();

            await _unitofWork.CartRepository.DeleteCartAsync(orderDTO.CartId);

            return order;
        }


        public async Task<IReadOnlyList<ReturnOrderDTO>> GetAllOrdersForUserAsync(string CustomerCode)
        {
            var orders = await _context.Orders.Where(o => o.CustomerCode == CustomerCode)
                .Include(o => o.OrderDetails)
                .Include(o => o.Shippings)
                .ToListAsync();

            var result = _mapper.Map<IReadOnlyList<ReturnOrderDTO>>(orders);
            return result;
        }

        public async Task<ReturnOrderDTO> GetOrderByIdAsync(string orderNumber, string CustomerCode)
        {
            var order = await _context.Orders.Where(o => o.OrderNumber == orderNumber && o.CustomerCode == CustomerCode)
                .Include(o => o.OrderDetails)
                .Include(o => o.Shippings)
                .FirstOrDefaultAsync(); 
            if (order == null)
            {
                throw new InvalidOperationException("Order not found.");
            }

            var result = _mapper.Map<ReturnOrderDTO>(order);
            return result;
        }
    }
}
