using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.core.Entities;
using Talabat.core.Entities.OrderAggregate;

namespace Talabat.core.IServices
{
    public interface IPaymentService
    {
        Task<CustomerBasket> CreateOrUpdatePaymentIntentAsync(string basketId);

        Task<Order> UpdatePaymentIntentSucceededOrFailed( string paymentIntentId, bool IsSuccess);
    }
}
