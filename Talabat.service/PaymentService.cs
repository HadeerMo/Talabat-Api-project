using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.core.Entities;
using Talabat.core.Entities.OrderAggregate;
using Talabat.core.IRepositories;
using Talabat.core.IServices;
using Talabat.core.Specifications;
using Product = Talabat.core.Entities.Product;

namespace Talabat.service
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(IConfiguration configuration, IBasketRepository basketRepository, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<CustomerBasket> CreateOrUpdatePaymentIntentAsync(string basketId)
        {
            StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];
            var basket = await _basketRepository.GetBasketAsync(basketId);
            if (basket == null) return null;
            var shippingPrice = 0m;
            if (basket.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(basket.DeliveryMethodId.Value);
                shippingPrice = deliveryMethod.Cost;
                basket.ShippingPrice = shippingPrice;
            }
            foreach(var item in basket.Items)
            {
                var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                if (item.Price != product.Price)
                    item.Price = product.Price;
            }
            var service = new PaymentIntentService();
            PaymentIntent intent;
            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions()
                {
                    Amount = (long)basket.Items.Sum(item => item.Quantity * (item.Price * 100)) + (long)shippingPrice,
                    Currency = "usd",
                    PaymentMethodTypes = new List<string>() { "card" }
                };
                intent = await service.CreateAsync(options);
                basket.PaymentIntentId = intent.Id;
                basket.ClientSecret = intent.ClientSecret;
            }
            else
            {
                var options = new PaymentIntentUpdateOptions()
                {
                    Amount = (long)basket.Items.Sum(item => item.Quantity * (item.Price * 100)) + (long)shippingPrice
                };
                await service.UpdateAsync(basket.PaymentIntentId,options);
            }

            await _basketRepository.UpdateBasketAsync(basket);
            return basket;

        }

        public async Task<Order> UpdatePaymentIntentSucceededOrFailed(string paymentIntentId, bool IsSuccess)
        {
            var spec = new OrderWithPaymentIntentIdSpecification(paymentIntentId);
            var order = await _unitOfWork.Repository<Order>().GetByIdWithSpecAsync(spec);
            if (IsSuccess)
                order.Status = OrderStatus.PaymentReceived;
            else
                order.Status = OrderStatus.PaymentFaild;

            _unitOfWork.Repository<Order>().Update(order);
            await _unitOfWork.Complete();
            return order;
        }
    }
}
