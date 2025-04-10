using EcommerceGraduation.Core.DTO;
using EcommerceGraduation.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceGraduation.Core.Interfaces
{
    public interface ICustomerRepository
    {
        Task<CustomerDTO> GetCustomerByIdAsync(string Id);
        Task<string> DeleteCustomerAsync(string Id);
        Task<CustomerDTO> UpdateCustomerAsync(string Id, CustomerDTO customerDTO);
    }
}
