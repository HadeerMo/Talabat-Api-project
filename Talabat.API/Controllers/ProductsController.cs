using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Talabat.API.Dtos;
using Talabat.API.Helpers;
using Talabat.core.Entities;
using Talabat.core.IRepositories;
using Talabat.core.Specifications;

namespace Talabat.API.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        //private readonly IGenericRepository<Product> _productRepo;
        //private readonly IGenericRepository<ProductBrand> _brandRepo;
        //private readonly IGenericRepository<ProductType> _typeRepo;
        private readonly IMapper _mapper;

        public ProductsController(IUnitOfWork unitOfWork,
                                  //IGenericRepository<Product> productRepo,
                                  //IGenericRepository<ProductBrand> brandRepo,
                                  //IGenericRepository<ProductType> typeRepo,
                                  IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            //_productRepo = productRepo;
            //_brandRepo = brandRepo;
            //_typeRepo = typeRepo;
            _mapper = mapper;
        }
        [CachedAttribute(600)]
        [HttpGet]
        public async Task<ActionResult<Pagination<ProductToReturnDTO>>> GetAllProducts([FromQuery]ProductSpecParams productParams)
        {
            var spec = new ProductWithBrandAndTypeSpecification(productParams);
            var products = await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(spec);
            var Data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDTO>>(products);
            var countSpec = new ProductWithFiltersForCountSpecification(productParams);
            var count = await _unitOfWork.Repository<Product>().GetCountAsync(countSpec);
            return Ok(new Pagination<ProductToReturnDTO>(productParams.PageIndex,productParams.PageSize, count, Data));
        }

        [CachedAttribute(600)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductToReturnDTO>> GetProductById(int id)
        {
            var spec = new ProductWithBrandAndTypeSpecification(id);
            var product= await _unitOfWork.Repository<Product>().GetByIdWithSpecAsync(spec);
            return Ok(_mapper.Map<Product,ProductToReturnDTO>(product));
        }

        [CachedAttribute(600)]
        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrands()
        {
            var brands = await _unitOfWork.Repository<ProductBrand>().GetAllAsync();
            return Ok(brands);
        }

        [CachedAttribute(600)]
        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetTypes()
        {
            var types = await _unitOfWork.Repository<ProductType>().GetAllAsync();
            return Ok(types);
        }
    }
}
