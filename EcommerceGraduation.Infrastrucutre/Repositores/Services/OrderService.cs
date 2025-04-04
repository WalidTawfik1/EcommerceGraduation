using AutoMapper;
using EcommerceGraduation.Core.DTO;
using EcommerceGraduation.Core.Entities;
using EcommerceGraduation.Core.Interfaces;
using EcommerceGraduation.Core.Services;
using EcommerceGraduation.Core.Sharing;
using EcommerceGraduation.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
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
        private readonly IInvoiceService _invoiceService;

        public OrderService(IUnitofWork unitofWork, EcommerceDbContext context, IMapper mapper, IInvoiceService invoiceService)
        {
            _unitofWork = unitofWork;
            _context = context;
            _mapper = mapper;
            _invoiceService = invoiceService;
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

            // Check if user wants to use their profile address
            if (orderDTO.ShippingDTO.UseProfileAddress)
            {
                var customer = await _unitofWork.CustomerRepository.GetCustomerByIdAsync(CustomerCode);
                if (customer == null)
                {
                    throw new InvalidOperationException("Customer not found.");
                }
                if (string.IsNullOrEmpty(customer.Address) ||
                    string.IsNullOrEmpty(customer.City) ||
                    string.IsNullOrEmpty(customer.Country))
                {
                    throw new InvalidOperationException("No address found in your profile. Please provide a shipping address.");
                }
                shipping.Address = customer.Address;
                shipping.City = customer.City;
                shipping.Country = customer.Country;
                shipping.PostalCode = customer.PostalCode;
            }
            else
            {
                shipping.Address = orderDTO.ShippingDTO.Address;
                shipping.City = orderDTO.ShippingDTO.City;
                shipping.Country = orderDTO.ShippingDTO.Country;
                shipping.PostalCode = orderDTO.ShippingDTO.PostalCode;
            }

            // If the user wants to update their profile with this address
            if (orderDTO.ShippingDTO.UpdateProfileAddress)
            {
                var customer = await _unitofWork.CustomerRepository.GetCustomerByIdAsync(CustomerCode);
                if (customer != null)
                {
                    customer.Address = orderDTO.ShippingDTO.Address;
                    customer.City = orderDTO.ShippingDTO.City;
                    customer.Country = orderDTO.ShippingDTO.Country;
                    customer.PostalCode = orderDTO.ShippingDTO.PostalCode;

                    await _unitofWork.CustomerRepository.UpdateCustomerAsync(CustomerCode, customer);
                }
            }

            if (shipping.ShippingMethod == ShippingMethod.Standard || shipping.ShippingMethod == ShippingMethod.عادي)
            {
                shipping.EstimatedDeliveryDate = DateOnly.FromDateTime(DateTime.Now.AddDays(4));
                shipping.ShippingCost = 20m;
            }
            else if (shipping.ShippingMethod == ShippingMethod.Express || shipping.ShippingMethod == ShippingMethod.سريع)
            {
                shipping.EstimatedDeliveryDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2));
                shipping.ShippingCost = 40m;
            }
            else
            {
                shipping.EstimatedDeliveryDate = DateOnly.FromDateTime(DateTime.Now.AddDays(4));
                shipping.ShippingCost = 20m;
            }
            shipping.TrackingNumber = Guid.NewGuid().ToString();
            await _context.Shippings.AddAsync(shipping);
            await _context.SaveChangesAsync();

            shipping.TrackingNumber = GenerateCode.GenerateTrackingNumber(shipping.ShippingId);
            _context.Shippings.Update(shipping);

            shipping.OrderNumber = order.OrderNumber;

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            order.TotalAmount += shipping.ShippingCost;
            await _context.SaveChangesAsync();

            await _unitofWork.CartRepository.DeleteCartAsync(orderDTO.CartId);
            await _invoiceService.GenerateInvoiceAsync(order.OrderNumber);

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
