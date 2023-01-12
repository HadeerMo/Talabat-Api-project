using AutoMapper;
using Microsoft.Extensions.Configuration;
using Talabat.API.Dtos;
using Talabat.core.Entities.OrderAggregate;

namespace Talabat.API.Helpers
{
    public class OrderItemPictureUrlResolver : IValueResolver<OrderItem, OrderItemDto, string>
    {
        public IConfiguration Configuration { get; }

        public OrderItemPictureUrlResolver(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public string Resolve(OrderItem source, OrderItemDto destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.Product.PicturUrl))
                return $"{Configuration["ApiBaseUrl"]}{source.Product.PicturUrl}";
            return null;
        }
    }
}
