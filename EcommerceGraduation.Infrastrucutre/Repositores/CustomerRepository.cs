using AutoMapper;
using EcommerceGraduation.Core.DTO;
using EcommerceGraduation.Core.Entities;
using EcommerceGraduation.Core.Interfaces;
using EcommerceGraduation.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceGraduation.Infrastrucutre.Repositores
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly EcommerceDbContext _context;
        private readonly IMapper _mapper;

        public CustomerRepository(EcommerceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }



        public async Task<bool> DeleteCustomerAsync(string Id)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(Id);
                if (customer == null)
                {
                    return false;
                }
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException;
                var innerExceptionMessage = innerException?.Message ?? "No inner exception details";

                Console.WriteLine($"Database update error: {ex.Message}");
                Console.WriteLine($"Inner exception: {innerExceptionMessage}");

                throw;
            }
        }

        public async Task<CustomerDTO> GetCustomerByIdAsync(string Id)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Id == Id);
            return _mapper.Map<CustomerDTO>(customer);
        }

        public async Task<CustomerDTO> UpdateCustomerAsync(string customerId, CustomerDTO customerDTO)
        {
            if (string.IsNullOrEmpty(customerId))
            {
                throw new ArgumentException("Customer ID cannot be null or empty.");
            }

            // Get the actual entity from the database
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Id == customerId);
            if (customer == null)
            {
                throw new KeyNotFoundException($"Customer with ID {customerId} not found.");
            }

            // Map the DTO to the entity
            _mapper.Map(customerDTO, customer);

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return the updated DTO
            return _mapper.Map<CustomerDTO>(customer);
        }
    }
}
