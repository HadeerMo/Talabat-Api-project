using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.core.Entities.OrderAggregate
{
    public class Order:BaseEntity
    {
        public Order()
        {

        }
        public Order(string buyerEmail, Address shippingAddress, DeliveryMethod deliveryMethod, ICollection<OrderItem> items, decimal subTotal, string paymentIntentId)
        {
            BuyerEmail = buyerEmail;
            ShippingAddress = shippingAddress;
            DeliveryMethod = deliveryMethod;
            Items = items;
            SubTotal = subTotal;
            PaymentIntentId = paymentIntentId;
        }

        public string BuyerEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public Address ShippingAddress { get; set; } 
        public DeliveryMethod DeliveryMethod { get; set; } //Navigational property
        public ICollection<OrderItem> Items { get; set; } //Navigational property
        public Decimal SubTotal { get; set; }
        public string PaymentIntentId { get; set; }

        public decimal GetTotal()
            => SubTotal + DeliveryMethod.Cost;

    }
}
