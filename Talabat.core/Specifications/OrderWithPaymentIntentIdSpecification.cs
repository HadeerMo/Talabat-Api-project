using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.core.Entities.OrderAggregate;

namespace Talabat.core.Specifications
{
    public class OrderWithPaymentIntentIdSpecification:BaseSpecification<Order>
    {
        public OrderWithPaymentIntentIdSpecification(string paymentintentId):base(o=>o.PaymentIntentId==paymentintentId)
        {
        }
    }
}
