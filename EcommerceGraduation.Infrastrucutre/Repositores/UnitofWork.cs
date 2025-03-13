using AutoMapper;
using EcommerceGraduation.Core.Interfaces;
using EcommerceGraduation.Core.Services;
using EcommerceGraduation.Infrastructure.Data;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceGraduation.Infrastrucutre.Repositores
{
    public class UnitofWork : IUnitofWork
    {
        private readonly EcommerceDbContext _context;
        private readonly IProductImageManagmentService _imageManagmentService;
        private readonly IMapper _mapper;
        private readonly IConnectionMultiplexer _redis;

        public ICategoryRepository CategoryRepository { get; }

        public IProductRepository ProductRepository { get; }

        public IProductImageRepository ProductImageRepository { get; }

        public ICartRepository CartRepository { get; }

        public UnitofWork(EcommerceDbContext context, IProductImageManagmentService imageManagmentService, IMapper mapper,
            IConnectionMultiplexer redis)
        {
            _context = context;
            _imageManagmentService = imageManagmentService;
            _mapper = mapper;
            _redis = redis;
            CategoryRepository = new CategoryRepository(_context);
            ProductRepository = new ProductRepository(_context, _mapper, _imageManagmentService);
            ProductImageRepository = new ProductImageRepository(_context);
            CartRepository = new CartRepository(_redis);

        }
    }
}
