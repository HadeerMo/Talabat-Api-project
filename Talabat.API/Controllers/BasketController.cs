using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Talabat.API.Dtos;
using Talabat.core.Entities;
using Talabat.core.IRepositories;

namespace Talabat.API.Controllers
{
    public class BasketController : BaseApiController
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;

        public BasketController(IBasketRepository basketRepository,IMapper mapper)
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<CustomerBasket>> GetBasketById(string id)
        { 
            var basket = await _basketRepository.GetBasketAsync(id);
            return Ok(basket?? new CustomerBasket(id));
        }
        [HttpPost]
        public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasketDto basket)
        {
            var mappedBasket = _mapper.Map<CustomerBasketDto, CustomerBasket>(basket);
            var updateOrCreateBasket = await _basketRepository.UpdateBasketAsync(mappedBasket);
            return Ok(updateOrCreateBasket);
        }
        [HttpDelete]
        public async Task DeleteBasket(string id)
        { 
            await _basketRepository.DeleteBasketAsync(id);
        }
    }
}
