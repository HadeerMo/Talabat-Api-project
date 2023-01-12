using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.core.Entities.OrderAggregate;

namespace Talabat.core.Specifications
{
    public class OrderWithItemsAndDeliveryMethodSpec:BaseSpecification<Order>
    {
        public OrderWithItemsAndDeliveryMethodSpec(string buyerEmail) :base(o =>o.BuyerEmail == buyerEmail)
        {
            Includes.Add(o => o.DeliveryMethod);
            Includes.Add(o => o.Items);
            AddOrderByDescending(o => o.OrderDate);
        }
        public OrderWithItemsAndDeliveryMethodSpec(string buyerEmail,int orderId) : base(o => o.BuyerEmail == buyerEmail && o.Id==orderId)
        {
            Includes.Add(o => o.DeliveryMethod);
            Includes.Add(o => o.Items);
        }
    }
}
