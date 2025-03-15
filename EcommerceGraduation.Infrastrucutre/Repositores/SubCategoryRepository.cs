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
    public class SubCategoryRepository : GenericRepository<SubCategory, string>, ISubCategoryRepository
    {
        public SubCategoryRepository(EcommerceDbContext context) : base(context)
        {
        }
    }
}
