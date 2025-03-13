using EcommerceGraduation.Core.Entities;
using EcommerceGraduation.Core.Interfaces;
using EcommerceGraduation.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceGraduation.Infrastrucutre.Repositores
{
    public class CategoryRepository : GenericRepository<Category,string>, ICategoryRepository
    {
        public CategoryRepository(EcommerceDbContext context) : base(context)
        {
        }
    }
}
