using AutoMapper;
using EcommerceGraduation.Core.Entities;
using EcommerceGraduation.Core.Interfaces;
using EcommerceGraduation.Core.Services;
using EcommerceGraduation.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<Customer> _userManager;
        private readonly IEmailService _emailService;
        private readonly SignInManager<Customer> _signInManager;
        private readonly IGenerateToken _generateToken;

        public ICategoryRepository CategoryRepository { get; }

        public IProductRepository ProductRepository { get; }

        public IProductImageRepository ProductImageRepository { get; }

        public ICartRepository CartRepository { get; }

        public ISubCategoryRepository SubCategoryRepository { get; }

        public IBrandRepository BrandRepository { get; }

        public IAuthentication Authentication { get; }

        public ICustomerRepository CustomerRepository { get; }

        public IWishlistRepository WishlistRepository { get; }

        public UnitofWork(EcommerceDbContext context, IProductImageManagmentService imageManagmentService, IMapper mapper,
            IConnectionMultiplexer redis, UserManager<Customer> userManager, IEmailService emailService, SignInManager<Customer> signInManager, IGenerateToken generateToken)
        {
            _context = context;
            _imageManagmentService = imageManagmentService;
            _mapper = mapper;
            _redis = redis;
            _userManager = userManager;
            _emailService = emailService;
            _signInManager = signInManager;
            _generateToken = generateToken;

            CategoryRepository = new CategoryRepository(_context);
            ProductRepository = new ProductRepository(_context, _mapper, _imageManagmentService);
            ProductImageRepository = new ProductImageRepository(_context);
            CartRepository = new CartRepository(_redis,ProductRepository);
            SubCategoryRepository = new SubCategoryRepository(_context);
            BrandRepository = new BrandRepositroy(_context);
            Authentication = new AuthenticationRepository(_userManager, _emailService, _signInManager, _generateToken,context);
            CustomerRepository = new CustomerRepository(_context, _mapper);
            WishlistRepository = new WishlistRepository(_redis, ProductRepository);

        }
    }
}
