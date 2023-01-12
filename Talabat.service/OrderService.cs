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

namespace Talabat.service
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;

        //private readonly IGenericRepository<Product> _productRepo;
        //private readonly IGenericRepository<DeliveryMethod> _deliveryMethodRepo;
        //private readonly IGenericRepository<Order> _orderRepo;

        public OrderService(IBasketRepository basketRepository,
                            //IGenericRepository<Product> productRepo,
                            //IGenericRepository<DeliveryMethod> deliveryMethodRepo,
                            //IGenericRepository<Order> orderRepo,
                            IUnitOfWork unitOfWork,
                            IPaymentService paymentService
            )
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
            //_productRepo = productRepo;
            //_deliveryMethodRepo = deliveryMethodRepo;
            //_orderRepo = orderRepo;
        }
        public async Task<Order> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, Address ShippingAddress)
        {
            //1. Get basket from basket Repo
            var basket = await _basketRepository.GetBasketAsync(basketId);
            //2. Get selected Items at basket from products repo
            var orderitems = new List<OrderItem>();
            foreach(var item in basket.Items)
            {
                var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                var productItemOrder = new ProductItemOrder(product.Id, product.Name, product.PictureUrl);
                var orderItem = new OrderItem(product.Price, item.Quantity, productItemOrder);
                orderitems.Add(orderItem);
            }
            //3. Calculate SubTotal
            var subTotal = orderitems.Sum(item=>item.Price * item.Quantity);
            //4. Get delivery method from DeliveryMethods Repo
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);
            //5. Create order
            var spec = new OrderWithPaymentIntentIdSpecification(basket.PaymentIntentId);
            var existingOrder = await _unitOfWork.Repository<Order>().GetByIdWithSpecAsync(spec);
            if(existingOrder != null)
            {
                _unitOfWork.Repository<Order>().Delete(existingOrder);
                await _paymentService.CreateOrUpdatePaymentIntentAsync(basket.Id);
            }

            var order = new Order(buyerEmail, ShippingAddress, deliveryMethod, orderitems, subTotal, basket.PaymentIntentId);
            await _unitOfWork.Repository<Order>().CreateAsync(order);
            //6. Update database
            var result = await _unitOfWork.Complete();
            if (result <= 0) return null;
            return order;
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();
            return deliveryMethod;
        }

        public async Task<Order> GetOrderByIdForUserAsync(int orderId, string buyerEmail)
        {
            var spec = new OrderWithItemsAndDeliveryMethodSpec(buyerEmail, orderId);
            var order = await _unitOfWork.Repository<Order>().GetByIdWithSpecAsync(spec);
            return order;
        }

        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
            var spec = new OrderWithItemsAndDeliveryMethodSpec(buyerEmail);
            var orders = await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(spec);
            return orders;
        }
    }
}
